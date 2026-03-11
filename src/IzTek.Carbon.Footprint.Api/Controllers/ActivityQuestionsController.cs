using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Delete;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.SendPush;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Update;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetById;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Wolverine;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/activity-questions")]
public class ActivityQuestionsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Belirli bir ankete ait tüm aktivite sorularını listeler.
    /// </summary>
    /// <remarks>
    /// Query parametresi: pollId (opsiyonel) — belirtilmezse tüm sorular gelir.
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] Guid pollId)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<ActivityQuestionResponse>>>(new GetActivityQuestionsQuery(pollId)));

    /// <summary>
    /// Belirli bir aktivite sorusunun detayını getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<ActivityQuestionResponse>>(new GetActivityQuestionByIdQuery(id)));

    /// <summary>
    /// Yeni bir aktivite sorusu oluşturur.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateActivityQuestionCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>
    /// Mevcut bir aktivite sorusunu günceller.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateActivityQuestionCommand command)
    {
        command.Id = id;
        return CreateActionResultInstance(await bus.InvokeAsync<Result>(command));
    }

    /// <summary>
    /// Bir aktivite sorusunu siler.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(new DeleteActivityQuestionCommand(id)));

    /// <summary>
    /// Admin'in sorularla ilgili kullanıcılara hatırlatıcı bildirim göndermesini sağlar.
    /// </summary>
    [HttpPost("{id:guid}/push-notification")]
    public async Task<IActionResult> SendPushAsync(Guid id, [FromBody] SendQuestionPushNotificationCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command with { QuestionId = id}));
}