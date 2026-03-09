using Iztek.Carbon.Footprint.Application.Features.AcitivityQuestions.Commands.Create;
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
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<ActivityQuestionResponse>>>(new GetActivityQuestionsQuery(id)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<ActivityQuestionResponse>>(new GetActivityQuestionByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateActivityQuestionCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(UpdateActivityQuestionCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(new DeleteActivityQuestionCommand(id)));

    /// <summary>
    /// Admin'in sorularla ilgili kullanıcılara hatırlatıcı bildirim göndermesini sağlar.
    /// </summary>
    [HttpPost("send-push")]
    public async Task<IActionResult> SendPushAsync(SendQuestionPushNotificationCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result>(command));
}