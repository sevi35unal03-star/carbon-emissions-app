using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetAllPollResults;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetHomePage;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-results")]
public class UserResultsController(IMessageBus bus, IStringLocalizer<Resource> localizer)
    : BaseController(localizer)
{
    [Authorize(Roles = "Admin")]
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyResultsAsync([FromQuery] GetUserDailyResultsQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<List<UserDailyResultResponse>>>(query));

    [HttpGet("poll-results")]
    public async Task<IActionResult> GetAllPollResultsAsync(
        [FromQuery] Guid pollSetId,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        Console.WriteLine($"=== CONTROLLER: pollSetId={pollSetId}, month={month}, year={year}");

        var query = new GetAllPollResultsQuery
        {
            PollSetId = pollSetId,
            Month = month,
            Year = year
        };

        var result = await bus.InvokeAsync<Result<List<PollResultSummaryDto>>>(query);

        Console.WriteLine($"=== RESULT: {result.IsSuccessful}, Count={result.Data?.Count}");

        return CreateActionResultInstance(result);
    }

    [Authorize] // Hem Admin hem User erişebilir (Handler içinde yetki kontrolü zaten yapılıyor)
    [HttpGet("poll-detail")]
    public async Task<IActionResult> GetUserPollDetailAsync(
    [FromQuery] Guid pollSetId,
    [FromQuery] int month,
    [FromQuery] int year,
    [FromQuery] Guid? targetUserId)
    {
        var query = new GetUserPollDetailQuery
        {
            PollSetId = pollSetId,
            Month = month,
            Year = year,
            TargetUserId = targetUserId
        };

        var result = await bus.InvokeAsync<Result<UserPollDetailResponse>>(query);
        return CreateActionResultInstance(result);
    }

    [Authorize]
    [HttpGet("home")]
    public async Task<IActionResult> GetHomePageAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetHomePageResponse>>(new GetHomePageQuery()));

    [Authorize]
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboardAsync([FromQuery] GetMonthlyLeaderboardQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetMonthlyLeaderboardResponse>>(query));
}