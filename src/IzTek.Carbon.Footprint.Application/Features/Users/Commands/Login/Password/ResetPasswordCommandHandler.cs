using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

public class ResetPasswordCommandHandler(UserManager<User> userManager)
{
    public async Task<Result> HandleAsync(
        ResetPasswordCommand request,
        CancellationToken ct)
    {
        var normalizedPhone = NormalizePhone(request.PhoneNumber);

        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == normalizedPhone, ct);

        if (user == null)
            return Result.Failure(SystemErrorCodes.UserNotFound, HttpStatusCode.NotFound);

        var savedCode = await userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");
        var savedExpiry = await userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry");
        var attemptStr = await userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetAttempts");

        int attempts = int.TryParse(attemptStr, out var a) ? a : 0;

        // LIMIT
        if (attempts >= 5)
        {
            await ClearOtp(user); // 🔥 kilitle
            return Result.Failure(SystemErrorCodes.TooManyRequests, HttpStatusCode.TooManyRequests);
        }

        // EXPIRE
        if (savedExpiry is null || !DateTime.TryParse(savedExpiry, out var expiry) || expiry < DateTime.UtcNow)
        {
            await ClearOtp(user);
            return Result.Failure(SystemErrorCodes.InvalidOtpCode, HttpStatusCode.BadRequest);
        }

        var hashedInput = HashOtp(request.ResetCode);

        if (savedCode == null || savedCode != hashedInput)
        {
            attempts++;
            await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetAttempts", attempts.ToString());

            return Result.Failure(SystemErrorCodes.InvalidOtpCode, HttpStatusCode.BadRequest);
        }

        // RESET PASSWORD
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var identityResult = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

        if (!identityResult.Succeeded)
            return Result.Failure(SystemErrorCodes.SystemError, HttpStatusCode.InternalServerError);

        await ClearOtp(user);
        await userManager.UpdateSecurityStampAsync(user);

        return Result.Success();
    }

    private async Task ClearOtp(User user)
    {
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry");
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetAttempts");
    }

    private string NormalizePhone(string phone)
    {
        phone = phone.Replace(" ", "").Replace("-", "");

        if (phone.StartsWith("0"))
            phone = "+90" + phone[1..];
        else if (phone.StartsWith("90"))
            phone = "+" + phone;

        return phone;
    }

    private string HashOtp(string otp)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(otp);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}