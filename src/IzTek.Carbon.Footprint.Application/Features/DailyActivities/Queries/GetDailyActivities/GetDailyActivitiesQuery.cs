namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetDailyActivities;

public record DailyActivityResponse(
    List<PendingQuestionDto> PendingQuestions,
    List<HistoryGroupDto> History); // ✅ CompletedActivityDto → HistoryGroupDto

public record HistoryGroupDto(
    string Date,
    List<CompletedActivityDto> Activities); // ✅ class yerine record

public record PendingQuestionDto(
    Guid Id,
    string Text,
    TimeSpan ScheduledTime,
    List<OptionDto> Options)
{
    // ✅ Record içinde property tanımlanabilir
    public string TimeRemaining =>
        ScheduledTime > DateTime.Now.TimeOfDay
            ? $"{(ScheduledTime - DateTime.Now.TimeOfDay):hh\\:mm} saat kaldı"
            : "Süre doldu";
}

public record OptionDto(
    Guid Id,
    string Text,
    Guid? NextQuestionId,
    bool IsFinalStep = false); // ✅ Default değer eklendi

public record CompletedActivityDto(
    string QuestionText,
    string AnswerText,
    double CarbonScore,  // ✅ Puan → CarbonScore
    DateTime Date);

public record GetDailyActivitiesQuery;