using Iztek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;
using Iztek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Password;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;
using IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    // --- Authentication & Password Management ---

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<LoginCommand>>(command));

    [AllowAnonymous]
    [HttpPost("password/forgot")]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    // --- User Queries ---

    [HttpGet("profile")]
    [EnableRateLimiting("user")]
    public async Task<IActionResult> GetProfileAsync()
        // Kullanıcı ID'si genellikle Token'dan (User.Identity) alınır, 
        // Query içinde bunu handle ettiğinizi varsayıyorum.
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetUserProfileResponse>>(new GetUserProfileQuery()));

    [HttpGet("detailed")]
    [Authorize(Roles = "Admin")] // Sadece adminlerin detaylı listeyi görebildiğini varsayalım
    public async Task<IActionResult> GetDetailedListAsync([FromQuery] GetUsersDetailedQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<GetUsersDetailedResponse>>>(query));

    /// <summary>
    /// Kullanıcının tüm puanlarını ağaç bağışına dönüştürür.
    /// </summary>
    [HttpPost("donate-trees")]
    public async Task<IActionResult> DonateTreesAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<DonateTreesResponse>>(new DonateTreesCommand()));

    /// <summary>
    /// Kullanıcının bağış geçmişini getirir.
    /// </summary>
    [HttpGet("donation-history")]
    public async Task<IActionResult> GetDonationHistoryAsync([FromQuery] GetDonationHistoryQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetDonationHistoryResponse>>(query));
}