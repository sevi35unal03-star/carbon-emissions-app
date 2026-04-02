using IzTek.Carbon.Footprint.Application.Features.Goals.Commands;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.CreateGlobal;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.DeleteGlobal;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.UpdateGlobal;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetHomePage;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;

namespace IzTek.Carbon.Footprint.Api.Controllers;

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

    [HttpGet("home")]
    public async Task<IActionResult> GetHomePageAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetHomePageResponse>>(new GetHomePageQuery()));

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboardAsync([FromQuery] GetMonthlyLeaderboardQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetMonthlyLeaderboardResponse>>(query));

    [Authorize(Roles = "Admin")]
    [HttpPost("global")]
    public async Task<IActionResult> CreateGlobalGoalAsync([FromBody] CreateGlobalGoalCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GlobalGoalResponse>>(command));

    [Authorize(Roles = "Admin")]
    [HttpPut("global/{id:guid}")]
    public async Task<IActionResult> UpdateGlobalGoalAsync(
        Guid id,
        [FromBody] UpdateGlobalGoalCommand command)
    {
        command = command with { Id = id };
        return CreateActionResultInstance(
            await bus.InvokeAsync<Result<GlobalGoalResponse>>(command));
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("global/{id:guid}")]
    public async Task<IActionResult> DeleteGlobalGoalAsync(Guid id, [FromQuery] int month, [FromQuery] int year)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result>(new DeleteGlobalGoalCommand(id, month, year)));
}