using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Update;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Delete;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetPreviousGoals;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

namespace IzTek.Carbon.Footprint.Api.Controllers;

// Admin: gunluk cevaplar
[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-results")]
public class UserResultsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyResultsAsync([FromQuery] GetUserDailyResultsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<UserDailyResultResponse>>>(query));

    // Figma: Ana Sayfa liderboard — tum kullanicilara acik, [Authorize] override eder Admin'i
    [Authorize]
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboardAsync([FromQuery] GetMonthlyLeaderboardQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetMonthlyLeaderboardResponse>>(query));
}

// Admin: hedef CRUD
[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/goals")]
public class GoalsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    [HttpGet]
    public async Task<IActionResult> GetGoalAsync([FromQuery] GetGoalDetailQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetGoalDetailResponse>>(query));

    [HttpGet("history")]
    public async Task<IActionResult> GetGoalHistoryAsync([FromQuery] GetPreviousGoalsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<GetPreviousGoalsResponse>>>(query));

    [HttpGet("yearly-summary")]
    public async Task<IActionResult> GetYearlySummaryAsync([FromQuery] GetYearlyGoalsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetYearlyGoalsResponse>>(query));

    [HttpPost]
    public async Task<IActionResult> CreateGoalAsync([FromBody] CreateGoalCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<CreateGoalResponse>>(command));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGoalAsync(Guid id, [FromBody] UpdateGoalCommand command)
     => CreateActionResultInstance(await bus.InvokeAsync<Result<UpdateGoalResponse>>(command));
    
    // month + year: cache invalidation icin zorunlu
    // Flutter: DELETE /goals/{id}?month=3&year=2026
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGoalAsync(Guid id, [FromQuery] int month, [FromQuery] int year)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(new DeleteGoalCommand(id, month, year)));
}

 
 