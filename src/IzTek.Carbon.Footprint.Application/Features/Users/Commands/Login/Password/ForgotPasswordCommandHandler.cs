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
    public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == command.PhoneNumber, ct);

        if (user is null)
        {
            logger.LogWarning("ForgotPassword failed: User not found → {PhoneNumber}", command.PhoneNumber);
            return Result.Failure(SystemErrorCodes.NotFound, HttpStatusCode.NotFound);
        }

        // Sadece DeviceToken'ı kaydet
        await userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetDeviceToken");
        await userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetDeviceToken", command.DeviceToken);

        logger.LogInformation("ForgotPassword DeviceToken saved → UserId: {UserId}", user.Id);
        return Result.Success();
    }
}