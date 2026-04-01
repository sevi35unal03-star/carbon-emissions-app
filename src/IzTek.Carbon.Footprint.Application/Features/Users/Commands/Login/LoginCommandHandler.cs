using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TokenResponse = IzTek.Carbon.Footprint.Application.Common.Models.TokenResponse;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;

public class LoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<LoginCommandHandler> logger)
{
    public async Task<Result<TokenResponse>> HandleAsync(
        LoginCommand command,
        CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(command.EmailorIdentityNumber)
                   ?? await userManager.FindByNameAsync(command.EmailorIdentityNumber);

        if (user is null || user.IsDeleted)
            return Result<TokenResponse>.Failure(SystemErrorCodes.InvalidCredentials, HttpStatusCode.Unauthorized);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordValid)
            return Result<TokenResponse>.Failure(SystemErrorCodes.InvalidCredentials, HttpStatusCode.Unauthorized);

        var token = await tokenService.CreateTokenAsync(user);

        // Session cookie — HttpOnly, Secure, 30 gün
        httpContextAccessor.HttpContext?.Response.Cookies.Append(
            "refresh_token",
            token.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

        return Result<TokenResponse>.Success(token);
    }
}