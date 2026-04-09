using IzTek.Carbon.Footprint.Application.Features.Polls.Commands.CopyQuestions;
using IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;
using IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;
using IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetPolls;
using IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/polls")]
public class PollsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    // USER

    /// <summary>O ay aktif anketi getirir. Kullanici ayda bir kez cevaplayabilir.</summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActivePollAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetMonthlyPollResponse>>(new GetMonthlyPollQuery()));

    /// <summary>Anket cevaplarini kaydeder ve karbon skoru hesaplar.</summary>
    [HttpPost("answers")]
    public async Task<IActionResult> SubmitAnswerAsync([FromBody] SubmitPollAnswerCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<SubmitPollAnswerResponse>>(command));

    /// <summary>Anket taslağını kaydeder. Kullanıcı anketi yarıda bırakıp devam edebilir.</summary>
    [HttpPost("draft")]
    public async Task<IActionResult> SaveDraftAsync([FromBody] SubmitPollAnswerCommand command)
    {
        var draftCommand = command with { IsDraft = true };
        return CreateActionResultInstance(
            await bus.InvokeAsync<Result<SubmitPollAnswerResponse>>(draftCommand));
    }

    // ADMIN

    /// <summary>Admin — tum anket setlerini listeler.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<PollSummaryResponse>>>(new GetPollsQuery()));

    /// <summary>Admin — belirli bir anket setinin sorularini ve seceneklerini getirir.</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<PollDetailResponse>>(new GetPollByIdQuery(id)));

    /// <summary>Admin — yeni bir anket seti olusturur.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePollSetCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>
    /// Admin — kaynak sorulari hedef ankete kopyalar.
    /// REST: POST /polls/{id}/questions
    /// Body: { "sourceQuestionIds": ["guid1", "guid2"] }
    /// </summary>
    [HttpPost("{pollSetId:guid}/questions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CopyQuestionsAsync(Guid pollSetId, [FromBody] CopyQuestionsToPollCommand command)
    {
        command.PollSetId = pollSetId;
        return CreateActionResultInstance(await bus.InvokeAsync<Result>(command));
    }
}