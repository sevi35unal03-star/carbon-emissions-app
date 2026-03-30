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
        // 1. Önce email ile ara
        var user = await userManager.FindByEmailAsync(command.EmailorIdentityNumber);

        // 2. Bulunamazsa TC kimlik no ile ara (UserName olarak kayıtlı)
        if (user is null)
            user = await userManager.FindByNameAsync(command.EmailorIdentityNumber);

        // 3. Kullanıcı yoksa veya silinmişse
        if (user is null || user.IsDeleted)
        {
            //Failure(string v, HttpStatusCode notFound)
            logger.LogWarning("Login failed: User not found → {Input}", command.EmailorIdentityNumber);
            return Result<TokenResponse>.Failure(SystemErrorCodes.ValidationError, HttpStatusCode.Unauthorized);
        }

        // 4. Şifre kontrolü
        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordValid)
        {
            logger.LogWarning("Login failed: Invalid password → UserId: {UserId}", user.Id);
            return Result<TokenResponse>.Failure(SystemErrorCodes.ValidationError, HttpStatusCode.Unauthorized);
        }

        // 5. Token oluştur
        var token = await tokenService.CreateTokenAsync(user);

        logger.LogInformation("Login successful → UserId: {UserId}", user.Id);

        return Result<TokenResponse>.Success(token);


    }
}