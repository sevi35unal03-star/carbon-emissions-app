using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

public class ForgotPasswordCommandHandler(
    UserManager<User> userManager,
    IPlatformService platformService,
    IConfiguration configuration,
    ILogger<ForgotPasswordCommandHandler> logger)
{
    public async Task<Result> Handle(
        ForgotPasswordCommand command,
        CancellationToken ct)
    {
        // 1. Kullanıcıyı telefon numarasıyla bul
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == command.PhoneNumber, ct);

        if (user is null)
        {
            logger.LogWarning("ForgotPassword failed: User not found → {PhoneNumber}", command.PhoneNumber);
            return Result.Failure(SystemErrorCodes.NotFound, HttpStatusCode.NotFound);
        }

        // 2. DeviceId validasyonu
        if (string.IsNullOrWhiteSpace(command.DeviceToken))
        {
            logger.LogWarning("ForgotPassword failed: DeviceToken missing → {PhoneNumber}", command.PhoneNumber);
            return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);
        }

        // 3. OTP kodu oluştur
        var resetCode = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
        var expiry = DateTime.UtcNow.AddMinutes(15).ToString("o");

        // 4. Eskiyi sil, yenisini kaydet (DeviceToken de dahil)
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry");
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetDeviceToken");

        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetOTP", resetCode);
        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry", expiry);
        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetDeviceToken", command.DeviceToken);
        // 5. Mock modda OTP log'a yazılır
        var useMock = configuration.GetValue<bool>("UseMockPlatformService");
        if (useMock)
        {
            logger.LogInformation("[MOCK] OTP: {OTP} → UserId: {UserId} | DeviceId: {DeviceId}",
                resetCode, user.Id, command.DeviceToken);
            return Result.Success();
        }

        // 6. Production'da SMS gönder
        var result = await platformService.SendSmsAsync(
            phoneNumber: user.PhoneNumber!,
            message: $"Şifre sıfırlama kodunuz: {resetCode}. Bu kod 15 dakika geçerlidir.");

        if (result is null || !result.IsSuccessful)
        {
            logger.LogError("ForgotPassword SMS failed → UserId: {UserId}", user.Id);
            return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);
        }

        logger.LogInformation("ForgotPassword OTP sent → UserId: {UserId} | DeviceToken: {DeviceToken}",
            user.Id, command.DeviceToken);
        return Result.Success();
    }
}
