using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetHomePage;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[ApiController]
[Route("api/v1/user-results")]
public class UserResultsController(IMessageBus bus, IStringLocalizer<Resource> localizer)
    : BaseController(localizer)
{
    [Authorize(Roles = "Admin")]
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyResultsAsync([FromQuery] GetUserDailyResultsQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<List<UserDailyResultResponse>>>(query));

    [AllowAnonymous]
    [HttpGet("home")]
    public async Task<IActionResult> GetHomePageAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetHomePageResponse>>(new GetHomePageQuery()));

    [AllowAnonymous]
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboardAsync([FromQuery] GetMonthlyLeaderboardQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetMonthlyLeaderboardResponse>>(query));
}