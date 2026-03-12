using Microsoft.AspNetCore.Identity;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

public class ResetPasswordCommandHandler(UserManager<User> userManager)
{
    public async Task<Result> HandleAsync(
        ResetPasswordCommand request, 
        CancellationToken ct)
    {
        // 1. Kullanıcıyı bul (Telefon numarası üzerinden)
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, ct);

        if (user == null)
            return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);

        // 2. Veritabanına (IdentityToken tablosuna) kaydettiğimiz 5 haneli kodu geri çek
        var savedCode = await userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");

        // 3. Kod doğruluğunu kontrol et
        if (savedCode == null || savedCode != request.ResetCode)
            return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);

        // 4. Identity'nin şifre sıfırlama mekanizmasını tetikle
        // Not: ResetPasswordAsync metodu bir 'ResetToken' bekler. Bu token'ı sistemden alıyoruz.
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var identityResult = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

        if (!identityResult.Succeeded)
        {
            return Result.Failure(SystemErrorCodes.SystemError, HttpStatusCode.ExpectationFailed);
        }

        // 5. GÜVENLİK: Kullanılan kodu veritabanından temizle (Tekrar kullanılamasın)
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");

        // 6. GÜVENLİK: Kullanıcının tüm aktif oturumlarını sonlandır (Opsiyonel)
        await userManager.UpdateSecurityStampAsync(user);

        return Result.Success();
    }
}