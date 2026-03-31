using Microsoft.AspNetCore.Identity;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

public class ResetPasswordCommandHandler(UserManager<User> userManager)
{
    public async Task<Result> HandleAsync(
     ResetPasswordCommand request,
     CancellationToken ct)
    {
        // 1. Kullanıcıyı bul
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, ct);

        if (user == null)
            return Result.Failure(SystemErrorCodes.UserNotFound, HttpStatusCode.NotFound);

        // 2. OTP kodunu çek
        var savedCode = await userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");

        // 3. Kod doğruluğunu kontrol et
        if (savedCode == null || savedCode != request.ResetCode)
            return Result.Failure(SystemErrorCodes.InvalidOtpCode, HttpStatusCode.BadRequest); // ← düzeltildi

        // 4. Şifre sıfırla
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var identityResult = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

        if (!identityResult.Succeeded)
            return Result.Failure(SystemErrorCodes.SystemError, HttpStatusCode.InternalServerError);

        // 5. OTP kodunu temizle
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");

        // 6. Tüm oturumları sonlandır
        await userManager.UpdateSecurityStampAsync(user);

        return Result.Success();
    }
}