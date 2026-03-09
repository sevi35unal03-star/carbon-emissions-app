using Iztek.Carbon.Footprint.Application.Features.Polls.Queries.GetDailyPoll;
using IzTek.Carbon.Footprint.Application.Features.Polls.Commands.CopyQuestions;
using IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;
using IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetDailyPoll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Wolverine;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/polls")]
public class PollsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    // --- USER ACTIONS ---

    /// <summary>
    /// Kullanıcının o ay cevaplaması gereken güncel anketi getirir.
    /// </summary>
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyPollAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<GetDailyPollResponse>>(new GetDailyPollQuery()));

    /// <summary>
    /// Kullanıcı anketi cevapladığında sonuçları kaydeder.
    /// </summary>
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitAnswerAsync(SubmitPollAnswerCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));


    // --- ADMIN ACTIONS ---

    /// <summary>
    /// Admin yeni bir anket seti (sorular ve seçenekler ile birlikte) oluşturur.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync(CreatePollSetRequest command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>
    /// Admin, mevcut/eski anket sorularını yeni bir döneme kopyalar.
    /// </summary>
    [HttpPost("copy-questions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CopyQuestionsAsync(CopyQuestionsToPollCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));
}