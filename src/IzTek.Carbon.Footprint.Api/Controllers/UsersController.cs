using IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    // AUTH

    /// <summary>BizIzmir uyeligi ile yeni kullanici kaydi olusturur.</summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<Guid>>(command));

    /// <summary>Kullanici girisi yapar ve JWT token doner.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<LoginCommand>>(command));

    /// <summary>Sifremi unuttum: e-posta/TC kimligine sifirlama linki gonderir.</summary>
    [AllowAnonymous]
    [HttpPost("password/forgot")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>Sifre sifirlama tokeni ile yeni sifreyi kaydeder.</summary>
    [AllowAnonymous]
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    // ME

    /// <summary>Token sahibi kullanicinin profil bilgilerini getirir.</summary>
    [HttpGet("me/profile")]
    [EnableRateLimiting("user")]
    public async Task<IActionResult> GetProfileAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetUserProfileResponse>>(new GetUserProfileQuery()));

    /// <summary>Token sahibi kullanicinin gecmis agac bagislarini listeler.</summary>
    [HttpGet("me/donations")]
    public async Task<IActionResult> GetDonationHistoryAsync([FromQuery] GetDonationHistoryQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetDonationHistoryResponse>>(query));

    /// <summary>Birikimli puanlari agac bagisina donusturur. Body gerekmez.</summary>
    [HttpPost("me/donations")]
    public async Task<IActionResult> DonateTreesAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<DonateTreesResponse>>(new DonateTreesCommand()));

    // ADMIN
    // REST: GET /users (eski: GET /users/all)

    /// <summary>Admin — tum kullanicilarin detayli listesini getirir.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetUsersDetailedQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<GetUsersDetailedResponse>>>(query));
}