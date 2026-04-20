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
    public async Task<Result> HandleAsync(
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

        // 2. OTP kodu oluştur
        var resetCode = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
        var expiry = DateTime.UtcNow.AddMinutes(15).ToString("o");

        // 3. Önce eskiyi sil (yoksa hata vermesin), sonra yenisini kaydet
        try
        {
            await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");
            await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry");
        }
        catch
        {
            // Kayıt yoksa sessizce geç
        }

        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetOTP", resetCode);
        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry", expiry);
        // 4. Mock modda OTP log'a yazılır
        var useMock = configuration.GetValue<bool>("UseMockPlatformService");
        if (useMock)
        {
            logger.LogInformation("[MOCK] OTP: {OTP} → UserId: {UserId}", resetCode, user.Id);
            return Result.Success();
        }

        // 5. Production'da SMS gönder
        var result = await platformService.SendSmsAsync(
            phoneNumber: user.PhoneNumber!,
            message: $"Şifre sıfırlama kodunuz: {resetCode}. Bu kod 15 dakika geçerlidir.");

        if (result is null || !result.IsSuccessful)
        {
            logger.LogError("ForgotPassword SMS failed → UserId: {UserId}", user.Id);
            return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);
        }

        logger.LogInformation("ForgotPassword OTP sent → UserId: {UserId}", user.Id);
        return Result.Success();
    }
}