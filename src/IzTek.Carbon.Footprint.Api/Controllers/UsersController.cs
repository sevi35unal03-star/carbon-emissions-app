using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Delete;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;
using IzTek.Carbon.Footprint.Infrastructure.Services;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(IMessageBus bus,
     ICurrentUserService currentUser,
     IStringLocalizer<Resource> localizer,
      ITokenService tokenService) : BaseController(localizer)
{
    // AUTH
   
    /// <summary>BizIzmir uyeligi ile yeni kullanici kaydi olusturur.</summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<Guid>>(command));

    /// <summary>Kullanici girisi yapar ve JWT token doner.</summary>
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

    /// <summary>Sifremi unuttum: e-posta/TC kimligine sifirlama linki gonderir.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("password/forgot")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand command)
    => CreateActionResultInstance(await bus.InvokeAsync<Result<string>>(command));

    /// <summary>Sifre sifirlama tokeni ile yeni sifreyi kaydeder.</summary>
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

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

    [Authorize]
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

    /// <summary>Token sahibi kullanicinin profil bilgilerini getirir.</summary>
    [HttpGet("me/profile")]
    [EnableRateLimiting("user")]
    public async Task<IActionResult> GetProfileAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetUserProfileResponse>>(new GetUserProfileQuery()));

    /// <summary>Token sahibi kullanıcının geçmiş ağaç bağışlarını listeler.</summary>
    [HttpGet("me/donations")]
    public async Task<IActionResult> GetDonationHistoryAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetDonationHistoryResponse>>(
                new GetDonationHistoryQuery()));  // ← UserId kaldırıldı

    /// <summary>Birikimli puanlari agac bagisina donusturur. Body gerekmez.</summary>
    ///

    /// <summary>
    /// Birikimli puanları ağaç bağışına dönüştürür.
    ///
    /// Seçenek 1 (tüm puan): POST /users/me/donations — body yok
    /// Seçenek 2 (kısmi):    POST /users/me/donations { "pointsToSpend": 5000 }
    /// </summary>
    [HttpPost("me/donations")]
    public async Task<IActionResult> DonateTreesAsync()
     => CreateActionResultInstance(
         await bus.InvokeAsync<Result<DonateTreesResponse>>(new DonateTreesCommand()));

    // ME

    // ... mevcut endpointler ...

    /// <summary>
    /// Token sahibi kullanıcının kendi profilini (hesabını) siler.
    /// Bu işlem geri alınamaz.
    /// </summary>
    /// <remarks>
    /// Hesap silme işlemi için onay zorunludur.
    /// </remarks>

    /// <summary>Hesabı siler (soft delete). Onay popup'ından sonra çağrılır.</summary>
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccountAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result>(new DeleteUserCommand()));

    //[HttpPost("me/donations")]
    // public async Task<IActionResult> DonateTreesAsync()
    // => CreateActionResultInstance(await bus.InvokeAsync<Result<DonateTreesResponse>>(new DonateTreesCommand()));

    // ADMIN
    // REST: GET /users (eski: GET /users/all)

    /// <summary>Admin — tum kullanicilarin detayli listesini getirir.</summary>
    /// 

    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
       => CreateActionResultInstance(Result.Success());

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetUsersDetailedQuery query)
    => CreateActionResultInstance(
        await bus.InvokeAsync<PagedResult<List<GetUsersDetailedResponse>>>(query));
}