namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.SubmitAnswer;

public record SubmitActivityAnswerCommand(
    Guid QuestionId,
    Guid SelectedOptionId,
    Guid UserId
);