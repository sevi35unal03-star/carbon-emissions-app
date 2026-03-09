namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class EmailSender(IPlatformService platformService) : IEmailSender<User>
{
    // 1. Hesap Onay Linki Gönderimi
    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        var content = $"Welcome {user.UserName}! Please confirm your account by clicking this link: {confirmationLink}";

        await platformService.SendEmailAsync(email, "Confırm emaıl",  content);
    }

    // 2. Şifre Sıfırlama Kodu Gönderimi (Genelde Mobil için)
    public async Task SendPasswordResetCodeAsync(User user, string phoneNumber, string resetCode)
    {
        var content = $"Your password reset code is: {resetCode}. It will expire in 30 minutes.";

        await platformService.SendEmailAsync( phoneNumber, "Confirm phoneNumber", content);
    }

    // 3. Şifre Sıfırlama Linki Gönderimi (Genelde Web için)
    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        var content = $"To reset your password, please click the following link: {resetLink}";

        await platformService.SendEmailAsync(email, "Confırm emaıl", content);
    }
}