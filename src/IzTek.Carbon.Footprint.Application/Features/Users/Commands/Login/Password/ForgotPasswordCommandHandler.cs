using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        var normalizedPhone = NormalizePhone(command.PhoneNumber);

        // EF Core custom metod çeviremez — önce çek, sonra filtrele
        var users = await userManager.Users
            .Where(u => u.PhoneNumber != null)
            .Select(u => new { u.Id, u.PhoneNumber })
            .ToListAsync(ct);

        var match = users.FirstOrDefault(u => NormalizePhone(u.PhoneNumber!) == normalizedPhone);

        if (match is null)
        {
            logger.LogWarning("ForgotPassword failed: User not found → {PhoneNumber}", normalizedPhone);
            return Result.Failure(SystemErrorCodes.NotFound, HttpStatusCode.NotFound);
        }

        var user = await userManager.FindByIdAsync(match.Id.ToString());

        if (user is null)
            return Result.Failure(SystemErrorCodes.NotFound, HttpStatusCode.NotFound);

        var now = DateTime.UtcNow;

        var resetCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var hashedOtp = HashOtp(resetCode);
        var expiry = now.AddMinutes(5).ToString("o");

        await ClearOtp(user);

        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetOTP", hashedOtp);
        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry", expiry);
        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetAttempts", "0");
        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetLastRequest", now.ToString("o"));

        var useMock = configuration.GetValue<bool>("UseMockPlatformService");

        if (useMock)
        {
            logger.LogInformation("[MOCK] OTP: {OTP} → UserId: {UserId}", resetCode, user.Id);
            return Result.Success();
        }

        var result = await platformService.SendSmsAsync(
            phoneNumber: user.PhoneNumber!,
            message: $"Şifre sıfırlama kodunuz: {resetCode}. Bu kod 5 dakika geçerlidir.");

        if (result is null || !result.IsSuccessful)
        {
            logger.LogError("ForgotPassword SMS failed → UserId: {UserId}", user.Id);
            return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);
        }

        return Result.Success();
    }

    private async Task ClearOtp(User user)
    {
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry");
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetAttempts");
    }

    private static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());

        if (digits.StartsWith("90") && digits.Length == 12)
            digits = digits[2..];
        else if (digits.StartsWith("0") && digits.Length == 11)
            digits = digits[1..];

        return digits;
    }

    private string HashOtp(string otp)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(otp);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}