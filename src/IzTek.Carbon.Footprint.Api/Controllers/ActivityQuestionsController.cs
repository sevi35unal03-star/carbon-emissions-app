using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Create;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Delete;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.SendPush;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Update;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetById;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetList;


namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/activity-questions")]
public class ActivityQuestionsController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>Aktivite sorularini listeler. pollId girilirse o ankete ait sorular gelir.</summary>
    /// <remarks>Query: pollId (opsiyonel)</remarks>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] Guid? pollId = null)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<ActivityQuestionResponse>>>(new GetActivityQuestionsQuery(pollId)));

    /// <summary>Belirli bir aktivite sorusunun detayini getirir.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<ActivityQuestionResponse>>(new GetActivityQuestionByIdQuery(id)));

    /// <summary>Yeni bir aktivite sorusu olusturur.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateActivityQuestionCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    /// <summary>Mevcut bir aktivite sorusunu gunceller.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateActivityQuestionCommand command)
    {
        command.Id = id;
        return CreateActionResultInstance(await bus.InvokeAsync<Result>(command));
    }

    /// <summary>Bir aktivite sorusunu siler.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(new DeleteActivityQuestionCommand(id)));

    /// <summary>
    /// Kullanicilara hatirlatici bildirim gonderir.
    /// REST: POST /activity-questions/{id}/notifications
    /// </summary>
    [HttpPost("{id:guid}/notifications")]
    public async Task<IActionResult> SendPushNotificationAsync(Guid id, [FromBody] SendQuestionPushNotificationCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));
    
}