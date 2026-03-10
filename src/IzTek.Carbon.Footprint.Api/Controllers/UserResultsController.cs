using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetPreviousGoals;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Update;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Delete;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-results")]
public class UserResultsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Admin için tüm kullanıcıların bugünkü karbon ayak izi özetini, 
    /// aktivite sayılarını ve bağışlanan ağaç sayılarını listeler.
    /// </summary>
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyResultsAsync([FromQuery] GetUserDailyResultsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<UserDailyResultResponse>>>(query));

    /// <summary>
    /// Admin için belirli bir kullanıcının, belirli bir aydaki anket detaylarını 
    /// (sorular, cevaplar ve skorlar) getirir.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: userId, month, year, userName
    /// </remarks>
    [HttpGet("poll-details")]
    public async Task<IActionResult> GetPollDetailsAsync([FromQuery] GetUserPollDetailQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<UserPollDetailResponse>>(query));

    /// <summary>
    /// Belirli bir aya ait hedef detayını getirir.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: month, year
    /// </remarks>
    [HttpGet("goal-detail")]
    public async Task<IActionResult> GetGoalDetailAsync([FromQuery] GetGoalDetailQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetGoalDetailResponse>>(query));

    /// <summary>
    /// Belirli bir aya ait aylık liderboard sıralamasını getirir.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: month, year
    /// </remarks>
    [HttpGet("monthly-leaderboard")]
    public async Task<IActionResult> GetMonthlyLeaderboardAsync([FromQuery] GetMonthlyLeaderboardQuery query)
     => CreateActionResultInstance(await bus.InvokeAsync<Result<GetMonthlyLeaderboardResponse>>(query));

    /// <summary>
    /// Belirli bir kullanıcının geçmiş hedeflerini listeler.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: userId
    /// </remarks>
    [HttpGet("previous-goals")]
    public async Task<IActionResult> GetPreviousGoalsAsync([FromQuery] GetPreviousGoalsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<GetPreviousGoalsResponse>>>(query));

    /// <summary>
    /// Belirli bir yıla ait aylık hedef listesini getirir.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: year
    /// </remarks>
    [HttpGet("yearly-goals")]
    public async Task<IActionResult> GetYearlyGoalsAsync([FromQuery] GetYearlyGoalsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetYearlyGoalsResponse>>(query));

    /// <summary>
    /// Yeni bir aylık hedef oluşturur.
    /// </summary>
    [HttpPost("goals")]
    public async Task<IActionResult> CreateGoalAsync([FromBody] CreateGoalCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<CreateGoalResponse>>(command));

    /// <summary>
    /// Mevcut bir hedefi günceller.
    /// </summary>
    [HttpPut("goals/{id}")]
    public async Task<IActionResult> UpdateGoalAsync(Guid id, [FromBody] UpdateGoalCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<UpdateGoalResponse>>(command with { Id = id }));

    [HttpDelete("goals/{id}")]
    public async Task<IActionResult> DeleteGoalAsync(Guid id, [FromQuery] int month, [FromQuery] int year)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(new DeleteGoalCommand(id, month, year)));
}