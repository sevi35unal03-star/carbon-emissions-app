using Iztek.Carbon.Footprint.Application.Features.DailyActivites.Queries.GetDailyQuestions;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.SubmitAnswer;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetActivityCalendar;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyActivityDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Wolverine;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/daily-activities")]
public class DailyActivitiesController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Admin'in "ActivityQuestions" altında oluşturduğu aktif soruları kullanıcı akışına getirir.
    /// </summary>
    [HttpGet("questions")]
    public async Task<IActionResult> GetDailyQuestionsAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<ActivityQuestionResponse>>>(new GetDailyQuestionsQuery()));

    /// <summary>
    /// Kullanıcının cevabını kaydeder ve "SubmitActivityAnswerResponse" döner (NextQuestion, Score vb.)
    /// </summary>
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitAnswerAsync(SubmitActivityAnswerCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<SubmitActivityAnswerResponse>>(command));

    /// <summary>
    /// Takvim görünümü için aktivite geçmişi.
    /// </summary>
    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendarAsync([FromQuery] GetActivityCalendarQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<CalendarResponse>>>(query));

    /// <summary>
    /// Belirli bir günün cevap detayları.
    /// </summary>
    [HttpGet("details")]
    public async Task<IActionResult> GetDetailsAsync([FromQuery] GetDailyActivityDetailsQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<DailyActivityDetailsResponse>>(query));
}