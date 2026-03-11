using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Password;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;


namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    // ──────────────────────────────────────────
    // AUTH
    // ──────────────────────────────────────────

    /// <summary>
    /// BizİZmir üyeliği ile yeni kullanıcı kaydı oluşturur.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<Guid>>(command));

    /// <summary>
    /// Kullanıcı girişi yapar ve JWT token döner.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<LoginCommand>>(command));

    /// <summary>
    /// Şifremi unuttum: kullanıcının e-postasına/TC'ye sıfırlama linki gönderir.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("password/forgot")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>
    /// Şifre sıfırlama tokenı ile yeni şifreyi kaydeder.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    // ──────────────────────────────────────────
    // ME  (token sahibi kullanıcı)
    // ──────────────────────────────────────────

    /// <summary>
    /// Token sahibi kullanıcının BizİZmir profil bilgilerini getirir.
    /// </summary>
    [HttpGet("me/profile")]
    [EnableRateLimiting("user")]
    public async Task<IActionResult> GetProfileAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetUserProfileResponse>>(new GetUserProfileQuery()));

    /// <summary>
    /// Token sahibi kullanıcının geçmiş ağaç bağışlarını listeler.
    /// </summary>
    [HttpGet("me/donations")]
    public async Task<IActionResult> GetDonationHistoryAsync([FromQuery] GetDonationHistoryQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetDonationHistoryResponse>>(query));

    /// <summary>
    /// Token sahibi kullanıcının birikmiş puanlarını ağaç bağışına dönüştürür.
    /// </summary>
    [HttpPost("me/donations")]
    public async Task<IActionResult> DonateTreesAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<DonateTreesResponse>>(new DonateTreesCommand()));

    // ──────────────────────────────────────────
    // ADMIN
    // ──────────────────────────────────────────

    /// <summary>
    /// Admin — tüm kullanıcıların detaylı listesini getirir.
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetUsersDetailedQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<GetUsersDetailedResponse>>>(query));
}