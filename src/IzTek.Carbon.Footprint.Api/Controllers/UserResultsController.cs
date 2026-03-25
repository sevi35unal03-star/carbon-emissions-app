using IzTek.Carbon.Footprint.Application.Features.Goals.Commands;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.CreateGlobal;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.DeleteGlobal;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.UpdateGlobal;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetHomePage;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetPreviousGoals;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-results")]
public class UserResultsController(IMessageBus bus, IStringLocalizer<Resource> localizer)
    : BaseController(localizer)
{
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyResultsAsync([FromQuery] GetUserDailyResultsQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<List<UserDailyResultResponse>>>(query));
}

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-results")]
public class UserResultsPublicController(IMessageBus bus, IStringLocalizer<Resource> localizer)
    : BaseController(localizer)
{
    [HttpGet("home")]
    public async Task<IActionResult> GetHomePageAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetHomePageResponse>>(new GetHomePageQuery()));

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboardAsync([FromQuery] GetMonthlyLeaderboardQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetMonthlyLeaderboardResponse>>(query));
}

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/goals")]
public class GoalsController(IMessageBus bus, IStringLocalizer<Resource> localizer)
    : BaseController(localizer)
{
    [HttpGet]
    public async Task<IActionResult> GetGoalAsync([FromQuery] GetGoalDetailQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetGoalDetailResponse>>(query));

    [HttpGet("history")]
    public async Task<IActionResult> GetGoalHistoryAsync([FromQuery] GetPreviousGoalsQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<List<GetPreviousGoalsResponse>>>(query));

    [HttpPost("global")]
    public async Task<IActionResult> CreateGlobalGoalAsync([FromBody] CreateGlobalGoalCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GlobalGoalResponse>>(command));

    [HttpPut("global/{id:guid}")]
    public async Task<IActionResult> UpdateGlobalGoalAsync(Guid id, [FromBody] UpdateGlobalGoalCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GlobalGoalResponse>>(command));

    [HttpDelete("global/{id:guid}")]
    public async Task<IActionResult> DeleteGlobalGoalAsync(Guid id, [FromQuery] int month, [FromQuery] int year)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result>(new DeleteGlobalGoalCommand(id, month, year)));
}