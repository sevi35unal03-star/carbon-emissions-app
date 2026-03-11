using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetActivityCalendar;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyActivityDetails;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPendingQuestions;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;

namespace IzTek.Carbon.Footprint.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/daily-activities")]
public class DailyActivitiesController(IMessageBus bus, IStringLocalizer<Resource> localizer) : BaseController(localizer)
{
    /// <summary>
    /// Kullanıcıya o gün için atanmış aktif aktivite sorularını getirir.
    /// Flutter bu listeyi alıp soru soru gösterir.
    /// </summary>
    [HttpGet("questions")]
    public async Task<IActionResult> GetDailyQuestionsAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<List<ActivityQuestionResponse>>>(new GetDailyQuestionsQuery()));

    /// <summary>
    /// Kullanıcının bir soruya verdiği cevabı kaydeder.
    /// Response'da NextQuestion ve kazanılan Score döner.
    /// </summary>
    [HttpPost("answers")]
    public async Task<IActionResult> SubmitAnswerAsync([FromBody] SubmitActivityAnswerCommand command)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<SubmitActivityAnswerResponse>>(command));

    /// <summary>
    /// Takvim görünümü için kullanıcının aktivite geçmişini getirir.
    /// month girilirse aylık, girilmezse yıllık tüm günler döner.
    /// </summary>
    /// <remarks>
    /// Query parametreleri: year (zorunlu), month (opsiyonel — girilmezse yıllık döner)
    /// </remarks>
    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendarAsync([FromQuery] GetActivityCalendarQuery query)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<CalendarResponse>>(query));

    /// <summary>
    /// Kullanıcının bugün cevaplaması gereken bekleyen soruları olup olmadığını döner.
    /// Flutter uygulama açılışında bunu kontrol ederek uyarı gösterir.
    /// Response: { hasPending: true, pendingCount: 2 }
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingAsync()
        => CreateActionResultInstance(await bus.InvokeAsync<Result<PendingQuestionsResponse>>(new GetPendingQuestionsQuery()));

    /// <summary>
    /// Belirli bir günün soru/cevap ve puan detaylarını getirir.
    /// Takvimde bir güne tıklandığında kullanılır.
    /// </summary>
    /// <remarks>
    /// Query parametresi: date (format: yyyy-MM-dd)
    /// </remarks>
    [HttpGet("details")]
    public async Task<IActionResult> GetDetailsAsync([FromQuery] DateTime date)
        => CreateActionResultInstance(await bus.InvokeAsync<Result<DailyActivityDetailsResponse>>(new GetDailyActivityDetailsQuery(date)));
}