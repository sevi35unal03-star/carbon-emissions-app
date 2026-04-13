using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetActivityCalendar;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyActivityDetails;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetMonthlyActivities;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPendingQuestions;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPreviousAnswers;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/daily-activities")]
public class DailyActivitiesController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Kullaniciya o gun icin atanmis aktif aktivite sorularini getirir.
    /// Flutter bu listeyi alip soru soru gosterir.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("questions")]
    public async Task<IActionResult> GetDailyQuestionsAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<List<DailyQuestionResponse>>>(
                new GetDailyQuestionsQuery()));

    /// <summary>
    /// Kullanicinin bir soruya verdigi cevabi kaydeder.
    /// Response'da NextQuestion ve kazanilan Score doner.
    /// </summary>
    [HttpPost("answers")]
    public async Task<IActionResult> SubmitAnswerAsync([FromBody] SubmitActivityAnswerCommand command)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<SubmitActivityAnswerResponse>>(command));

    /// <summary>
    /// Takvim gorunumu icin kullanicinin aktivite gecmisini getirir.
    /// month girilirse aylik, girilmezse yillik tum gunler doner.
    /// </summary>
    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendarAsync([FromQuery] GetActivityCalendarQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<CalendarResponse>>(query));

    /// <summary>
    /// Tümünü Gör — seçilen ay ve period (1-15 / 16-31) için günlük skor listesi.
    /// </summary>
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyActivitiesAsync([FromQuery] GetMonthlyActivitiesQuery query)
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<MonthlyActivityResponse>>(query));

    /// <summary>
    /// Gunluk Aktivitelerim — en son cevaplanmis gunun cevaplari.
    /// </summary>
    [HttpGet("previous-answers")]
    public async Task<IActionResult> GetPreviousAnswersAsync()
        => CreateActionResultInstance(
            await bus.InvokeAsync<Result<List<PreviousAnswerGroupDto>>>(
                new GetPreviousAnswersQuery()));

    /// <summary>
    /// REST unified endpoint:
    ///   GET /daily-activities?status=pending   -> bekleyen sorular
    ///   GET /daily-activities?date=yyyy-MM-dd  -> gun detayi
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] string? status,
        [FromQuery] DateTime? date)
    {
        if (status == "pending")
            return CreateActionResultInstance(
                await bus.InvokeAsync<Result<PendingQuestionsResponse>>(
                    new GetPendingQuestionsQuery()));

        if (date.HasValue)
            return CreateActionResultInstance(
                await bus.InvokeAsync<Result<DailyActivityDetailsResponse>>(
                    new GetDailyActivityDetailsQuery(date.Value)));

        return CreateActionResultInstance(
            Result.Failure(SystemErrorCodes.InvalidParameter, HttpStatusCode.BadRequest));
    }
}