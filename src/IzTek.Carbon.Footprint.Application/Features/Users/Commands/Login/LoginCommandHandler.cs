using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TokenResponse = IzTek.Carbon.Footprint.Application.Common.Models.TokenResponse;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;

public class LoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    ILogger<LoginCommandHandler> logger)
{
    public async Task<Result<TokenResponse>> HandleAsync(
        LoginCommand command,
        CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(command.EmailorIdentityNumber)
                   ?? await userManager.FindByNameAsync(command.EmailorIdentityNumber);

        if (user is null || user.IsDeleted)
        {
            logger.LogWarning("Login failed: User not found → {Input}", command.EmailorIdentityNumber);
            return Result<TokenResponse>.Failure(SystemErrorCodes.InvalidCredentials, HttpStatusCode.Unauthorized);
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordValid)
        {
            logger.LogWarning("Login failed: Invalid password → UserId: {UserId}", user.Id);
            return Result<TokenResponse>.Failure(SystemErrorCodes.InvalidCredentials, HttpStatusCode.Unauthorized);
        }

        logger.LogInformation("Login successful → UserId: {UserId}", user.Id);

        return Result<TokenResponse>.Success(await tokenService.CreateTokenAsync(user));
    }
}