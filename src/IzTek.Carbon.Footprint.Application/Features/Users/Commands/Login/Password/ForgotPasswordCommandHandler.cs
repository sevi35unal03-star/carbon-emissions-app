using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

public class ForgotPasswordCommandHandler(
    UserManager<User> userManager,
    IPlatformService platformService,
    IConfiguration configuration,  // ← eklendi
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
        var expiry = DateTime.UtcNow.AddMinutes(15).ToString("o"); // ISO 8601

        // 3. Her zaman DB'ye kaydet — reset handler buradan doğrulayacak
        await userManager.SetAuthenticationTokenAsync(
            user, "Default", "PasswordResetOTP", resetCode);

        await userManager.SetAuthenticationTokenAsync(
            user, "Default", "PasswordResetOTPExpiry", expiry); // ← süre

        // 4. Mock modda OTP response'da döner
        var useMock = configuration.GetValue<bool>("UseMockPlatformService");
        if (useMock)
        {
            logger.LogInformation("[MOCK] OTP: {OTP} → UserId: {UserId}", resetCode, user.Id);
            return Result<string>.Success(resetCode);  // ← OTP response'da
        }

        // 5. Production'da e-posta gönder
        var result = await platformService.SendEmailAsync(
            to: user.Email!,
            subject: "Şifre Sıfırlama Kodu",
            content: $"Şifre sıfırlama kodunuz: {resetCode}. Bu kod 15 dakika geçerlidir.");

        if (result is null || !result.IsSuccessful)
        {
            logger.LogError("ForgotPassword email failed → UserId: {UserId}", user.Id);
            return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);
        }

        logger.LogInformation("ForgotPassword OTP sent → UserId: {UserId}", user.Id);
        return Result.Success();
    }
}