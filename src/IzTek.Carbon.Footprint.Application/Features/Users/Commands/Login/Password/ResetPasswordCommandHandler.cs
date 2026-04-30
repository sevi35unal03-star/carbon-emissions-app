using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

    public class ResetPasswordCommandHandler(
    UserManager<User> userManager,
    IPlatformService platformService,
    IConfiguration configuration,
    ILogger<ResetPasswordCommandHandler> logger)
    {
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            // 1. Kullanıcıyı bul
            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, ct);

            if (user is null)
                return Result.Failure(SystemErrorCodes.UserNotFound, HttpStatusCode.NotFound);

            // 2. DeviceToken kontrolü
            var savedDeviceToken = await userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetDeviceToken");

            if (savedDeviceToken != request.DeviceToken)
            {
                logger.LogWarning("ResetPassword failed: DeviceToken mismatch → UserId: {UserId}", user.Id);
                return Result.Failure(SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);
            }

            // 3. OTP üret ve kaydet
            var resetCode = RandomNumberGenerator.GetInt32(10000, 99999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(15).ToString("o");

            await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTP");
            await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry");

            await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetOTP", resetCode);
            await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetOTPExpiry", expiry);

            // 4. SMS gönder
            var useMock = configuration.GetValue<bool>("UseMockPlatformService");
            if (useMock)
            {
                logger.LogInformation("[MOCK] OTP: {OTP} → UserId: {UserId}", resetCode, user.Id);
                return Result.Success();
            }

            var result = await platformService.SendSmsAsync(
                phoneNumber: user.PhoneNumber!,
                message: $"Şifre sıfırlama kodunuz: {resetCode}. Bu kod 15 dakika geçerlidir.");

            if (result is null || !result.IsSuccessful)
            {
                logger.LogError("ResetPassword SMS failed → UserId: {UserId}", user.Id);
                return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);
            }

            logger.LogInformation("ResetPassword OTP sent → UserId: {UserId}", user.Id);
            return Result.Success();
        }
    }
