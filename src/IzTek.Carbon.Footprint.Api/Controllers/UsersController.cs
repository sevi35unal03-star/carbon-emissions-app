using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Delete;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(
    IMessageBus bus,
    IStringLocalizer<Resource> localizer,
    ITokenService tokenService) : BaseController(localizer)
{
    // AUTH

    /// <summary>Yeni kullanıcı kaydı oluşturur.</summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>Kullanıcı girişi yapar ve JWT token döner.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command)
    {
        var result = await bus.InvokeAsync<Result<TokenResponse>>(command);

        if (result.IsSuccessful && result.Data?.RefreshToken is not null)
        {
            Response.Cookies.Append("refresh_token", result.Data.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        return CreateActionResultInstance(result);
    }

    /// <summary>Telefon numarasına 5 haneli OTP kodu gönderir.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("password/forgot")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>OTP kodu ile şifre sıfırlar.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>Refresh token ile yeni access token üretir.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("token/refresh")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(refreshToken))
            return CreateActionResultInstance(
                Result<TokenResponse>.Failure(SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized));

        var newToken = await tokenService.RefreshAccessTokenAsync(refreshToken);

        if (newToken is null)
            return CreateActionResultInstance(
                Result<TokenResponse>.Failure(SystemErrorCodes.SessionExpired, HttpStatusCode.Unauthorized));

        Response.Cookies.Append("refresh_token", newToken.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return CreateActionResultInstance(Result<TokenResponse>.Success(newToken));
    }

    /// <summary>Oturumu sonlandırır, refresh token'ı iptal eder.</summary>
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (!string.IsNullOrEmpty(refreshToken))
            await tokenService.RevokeRefreshTokenAsync(refreshToken, "Logout");

        Response.Cookies.Delete("refresh_token");

        return CreateActionResultInstance(Result.Success());
    }

    // ME

    /// <summary>Token sahibi kullanıcının profil bilgilerini getirir.</summary>
    [HttpGet("me/profile")]
    [EnableRateLimiting("user")]
    public async Task<IActionResult> GetProfileAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetUserProfileResponse>>(new GetUserProfileQuery()));

    /// <summary>Token sahibi kullanıcının geçmiş ağaç bağışlarını listeler.</summary>
    [HttpGet("me/donations")]
    public async Task<IActionResult> GetDonationHistoryAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetDonationHistoryResponse>>(new GetDonationHistoryQuery()));

    /// <summary>Birikimli puanların tamamını ağaç bağışına dönüştürür.</summary>
    [HttpPost("me/donations")]
    public async Task<IActionResult> DonateTreesAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<DonateTreesResponse>>(new DonateTreesCommand()));

    /// <summary>Hesabı siler (soft delete).</summary>
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccountAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result>(new DeleteUserCommand()));

    // ADMIN

    /// <summary>Admin — tüm kullanıcıların detaylı listesini getirir.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetUsersDetailedQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<PagedResult<List<GetUsersDetailedResponse>>>(query));
}