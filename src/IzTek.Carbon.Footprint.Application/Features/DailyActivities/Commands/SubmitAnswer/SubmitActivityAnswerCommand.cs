namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;

public record SubmitActivityAnswerCommand(
    Guid QuestionId,
    Guid SelectedOptionId,
    Guid UserId);