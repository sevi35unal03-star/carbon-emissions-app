using IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;
using IzTek.Carbon.Footprint.Application.Features.Polls.Commands.CopyQuestions;
using IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;
using IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetPolls;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/polls")]
public class PollsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    // ──────────────────────────────────────────
    // USER
    // ──────────────────────────────────────────

    /// <summary>
    /// Kullanıcının o ay cevaplaması gereken aktif anketi getirir.
    /// Kullanıcı ayda bir kez cevaplayabilir.
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActivePollAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetMonthlyPollResponse>>(new GetMonthlyPollQuery()));

    /// <summary>
    /// Kullanıcının anket cevaplarını kaydeder ve karbon skoru hesaplar.
    /// </summary>
    [HttpPost("answers")]
    public async Task<IActionResult> SubmitAnswerAsync([FromBody] SubmitPollAnswerCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>
    /// Anket sonuçlarını getirir.
    /// - Normal kullanıcı: token'dan userId alınır, sadece kendi sonucunu görebilir.
    /// - Admin: targetUserId query param ile istediği kullanıcının sonucuna ulaşabilir.
    /// Profil > Karbon Puanlarım ekranında kullanılır.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: month, year — Admin için ek olarak: targetUserId (opsiyonel)
    /// </remarks>
    [HttpGet("results")]
    public async Task<IActionResult> GetPollResultsAsync([FromQuery] GetUserPollDetailQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<UserPollDetailResponse>>(query));

    // ──────────────────────────────────────────
    // ADMIN
    // ──────────────────────────────────────────

    /// <summary>
    /// Admin — tüm anket setlerini listeler.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<PollSummaryResponse>>>(new GetPollsQuery()));

    /// <summary>
    /// Admin — belirli bir anket setinin sorularını ve seçeneklerini getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<PollDetailResponse>>(new GetPollByIdQuery(id)));

    /// <summary>
    /// Admin — yeni bir anket seti (sorular ve seçenekler ile birlikte) oluşturur.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePollSetCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>
    /// Admin — seçilen kaynak sorularını hedef ankete kopyalar.
    /// pollSetId: soruların kopyalanacağı hedef anketin id'si.
    /// Body'de kopyalanacak kaynak soru id listesi (sourceQuestionIds) gönderilir.
    /// Flutter örneği: POST /polls/{pollSetId}/copy-questions
    /// Body: { "sourceQuestionIds": ["guid1", "guid2"] }
    /// </summary>
    [HttpPost("{pollSetId:guid}/copy-questions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CopyQuestionsAsync(Guid pollSetId, [FromBody] CopyQuestionsToPollCommand command)
    {
        command.PollSetId = pollSetId;
        return CreateActionResultInstance(await bus.InvokeAsync<Result>(command));
    }
}