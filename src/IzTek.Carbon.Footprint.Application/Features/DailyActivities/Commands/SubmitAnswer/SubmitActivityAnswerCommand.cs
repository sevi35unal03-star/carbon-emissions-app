namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;

public class SubmitActivityAnswerCommand
{
    public List<ActivityAnswerDto> Answers { get; set; } = new();
}

public record ActivityAnswerDto(
    Guid QuestionId,
    Guid SelectedOptionId
);