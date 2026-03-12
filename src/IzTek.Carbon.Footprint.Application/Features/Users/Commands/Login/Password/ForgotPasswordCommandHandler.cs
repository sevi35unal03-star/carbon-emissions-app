using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

public class ForgotPasswordCommandHandler(
    UserManager<User> userManager,
    IPlatformService platformService,
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
            return Result.Failure(SystemErrorCodes.NotFound,HttpStatusCode.NotFound );
        }

        // 2. OTP kodu oluştur
        var resetCode = new Random().Next(10000, 99999).ToString();

        // 3. Kodu Identity'nin token tablosuna kaydet
        await userManager.SetAuthenticationTokenAsync(
            user,
            "Default",
            "PasswordResetOTP",
            resetCode);

        // 4. PlatformService ile email gönder
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