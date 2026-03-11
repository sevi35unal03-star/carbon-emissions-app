using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Update;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Delete;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetPreviousGoals;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;

namespace IzTek.Carbon.Footprint.Api.Controllers;

// ─────────────────────────────────────────────────────────────
// UserResultsController  →  Admin paneli için kullanıcı sonuçları
// ─────────────────────────────────────────────────────────────
[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-results")]
public class UserResultsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Admin paneli Günlük Cevaplar ekranı —
    /// tüm kullanıcıların bugünkü aktivite özetini, toplam puanlarını
    /// ve bağışladıkları ağaç sayısını listeler.
    /// </summary>
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyResultsAsync([FromQuery] GetUserDailyResultsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<UserDailyResultResponse>>>(query));

    /// <summary>
    /// Admin paneli Liderboard ekranı —
    /// belirli bir aya ait kullanıcı sıralama listesini getirir.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: month, year
    /// </remarks>
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboardAsync([FromQuery] GetMonthlyLeaderboardQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetMonthlyLeaderboardResponse>>(query));
}

// ─────────────────────────────────────────────────────────────
// GoalsController  →  Aylık hedef CRUD
// ─────────────────────────────────────────────────────────────
[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/goals")]
public class GoalsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Belirli bir aya ait hedefi getirir.
    /// month ve year zorunludur — girilmezse 400 döner.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: month (1-12), year (ör: 2026) — ikisi de zorunlu
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetGoalAsync([FromQuery] GetGoalDetailQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetGoalDetailResponse>>(query));

    /// <summary>
    /// Belirli bir kullanıcının tüm geçmiş hedeflerini listeler.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: userId
    /// </remarks>
    [HttpGet("history")]
    public async Task<IActionResult> GetGoalHistoryAsync([FromQuery] GetPreviousGoalsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<GetPreviousGoalsResponse>>>(query));

    /// <summary>
    /// Belirli bir yıla ait tüm aylık hedeflerin özetini getirir.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: year
    /// </remarks>
    [HttpGet("yearly-summary")]
    public async Task<IActionResult> GetYearlySummaryAsync([FromQuery] GetYearlyGoalsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetYearlyGoalsResponse>>(query));

    /// <summary>
    /// Yeni bir aylık hedef oluşturur.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateGoalAsync([FromBody] CreateGoalCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<CreateGoalResponse>>(command));

    /// <summary>
    /// Mevcut bir hedefi günceller.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGoalAsync(Guid id, [FromBody] UpdateGoalCommand command)
    => CreateActionResultInstance(await bus.InvokeAsync<Result<UpdateGoalResponse>>(command));

    /// <summary>
    /// Belirli bir hedefi siler.
    /// month ve year cache invalidation için zorunludur.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: month (1-12), year (ör: 2026) — ikisi de zorunlu
    /// Flutter örneği: DELETE /goals/{id}?month=3&amp;year=2026
    /// </remarks>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGoalAsync(Guid id, [FromQuery] int month, [FromQuery] int year)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(new DeleteGoalCommand(id, month, year)));
}