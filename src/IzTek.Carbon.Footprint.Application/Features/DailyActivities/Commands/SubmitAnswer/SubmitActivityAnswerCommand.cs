namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;

public record SubmitActivityAnswerCommand(
    Guid QuestionId,
    Guid SelectedOptionId,
    Guid UserId
) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
        [$"activity-calendar:{DateTime.Now.Year}:{DateTime.Now.Month}",
         $"activity-calendar:{DateTime.Now.Year}"];
}