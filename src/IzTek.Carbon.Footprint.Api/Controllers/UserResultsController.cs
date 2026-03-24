using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.CreateGlobal;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Delete;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.DeleteGlobal;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Update;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands.UpdateGlobal;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetHomePage;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetPreviousGoals;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;

namespace IzTek.Carbon.Footprint.Api.Controllers;

// ─── Admin: Günlük Cevaplar ───────────────────────────────────────────────────
[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-results")]
public class UserResultsController(IMessageBus bus, IStringLocalizer<Resource> localizer)
    : BaseController(localizer)
{
    /// <summary>Admin — kullanıcıların günlük aktivite cevaplarını listeler.</summary>
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyResultsAsync([FromQuery] GetUserDailyResultsQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<List<UserDailyResultResponse>>>(query));
}

// ─── Kullanıcıya Açık: Liderboard ve Ana Sayfa ───────────────────────────────
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/user-results")]
public class UserResultsPublicController(IMessageBus bus, IStringLocalizer<Resource> localizer)
    : BaseController(localizer)
{
    /// <summary>
    /// Ana Sayfa — Ulaşılması Hedeflenen Ağaç (genel + aylık) progress bilgisi.
    /// Flutter ana sayfa açılışında çağrır.
    /// </summary>
    [HttpGet("home")]
    public async Task<IActionResult> GetHomePageAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetHomePageResponse>>(new GetHomePageQuery()));

    /// <summary>
    /// Ayın Liderleri — Podium (#1,#2,#3) + liste + kullanıcının sırası.
    /// Ana sayfadaki "Detaya Git" buraya yönlendirir.
    /// </summary>
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboardAsync([FromQuery] GetMonthlyLeaderboardQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetMonthlyLeaderboardResponse>>(query));
}

// ─── Admin: Hedef CRUD ────────────────────────────────────────────────────────
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

    [HttpGet("yearly-summary")]
    public async Task<IActionResult> GetYearlySummaryAsync([FromQuery] GetYearlyGoalsQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<GetYearlyGoalsResponse>>(query));

    [HttpPost]
    public async Task<IActionResult> CreateGoalAsync([FromBody] CreateGoalCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<CreateGoalResponse>>(command));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGoalAsync(Guid id, [FromBody] UpdateGoalCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<UpdateGoalResponse>>(command));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGoalAsync(
        Guid id,
        [FromQuery] int month,
        [FromQuery] int year)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result>(new DeleteGoalCommand(id, month, year)));

    /// <summary>Admin — aylık global hedef belirler.</summary>
    [HttpPost("global")]
    public async Task<IActionResult> CreateGlobalGoalAsync(
        [FromBody] CreateGlobalGoalCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<CreateGoalResponse>>(command));

    /// <summary>Admin — aylık global hedefi günceller.</summary>
    [HttpPut("global/{id:guid}")]
    public async Task<IActionResult> UpdateGlobalGoalAsync(
        Guid id,
        [FromBody] UpdateGlobalGoalCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<UpdateGoalResponse>>(command));

    /// <summary>Admin — aylık global hedefi siler.</summary>
    /// <remarks>Flutter: DELETE /goals/global/{id}?month=3&year=2026</remarks>
    [HttpDelete("global/{id:guid}")]
    public async Task<IActionResult> DeleteGlobalGoalAsync(
        Guid id,
        [FromQuery] int month,
        [FromQuery] int year)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result>(
                new DeleteGlobalGoalCommand(id, month, year)));
}