using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

public class ResetPasswordCommandHandler(
    UserManager<User> userManager,
    ILogger<ResetPasswordCommandHandler> logger)  // logger eklendi
{
    public async Task<Result> Handle(
     ResetPasswordCommand request,
     CancellationToken ct)
    {
        // 1. Kullanıcıyı bul
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, ct);

        if (user == null)
            return Result.Failure(SystemErrorCodes.UserNotFound, HttpStatusCode.NotFound);

        // 2. Token'ları çek
        var savedCode = await userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");
        var savedExpiry = await userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry");
        var savedDeviceId = await userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetDeviceToken");

        // 3. DeviceId eşleşiyor mu?
        if (savedDeviceId != request.DeviceToken)
        {
            logger.LogWarning("OTP verify failed: DeviceId mismatch → UserId: {UserId}", user.Id);
            return Result.Failure(SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);
        }

        // 4. Süre kontrolü
        if (savedExpiry is null || DateTime.Parse(savedExpiry) < DateTime.UtcNow)
        {
            await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");
            await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry");
            await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetDeviceToken");
            return Result.Failure(SystemErrorCodes.InvalidOtpCode, HttpStatusCode.BadRequest);
        }

        // 5. Kod doğruluğunu kontrol et
        if (savedCode is null || savedCode != request.ResetCode)
            return Result.Failure(SystemErrorCodes.InvalidOtpCode, HttpStatusCode.BadRequest);

        // 6. Şifre sıfırla
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var identityResult = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

        if (!identityResult.Succeeded)
            return Result.Failure(SystemErrorCodes.SystemError, HttpStatusCode.InternalServerError);

        // 7. OTP token'larını temizle
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry");
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetDeviceToken");

        // 8. Tüm oturumları sonlandır
        await userManager.UpdateSecurityStampAsync(user);

        return Result.Success();
    }
}