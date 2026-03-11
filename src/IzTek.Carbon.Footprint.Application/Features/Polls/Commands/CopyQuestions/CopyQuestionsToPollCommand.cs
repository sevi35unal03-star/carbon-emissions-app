namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.CopyQuestions;

public class CopyQuestionsToPollCommand
{
    public Guid PollSetId { get; set; }
    public List<Guid> SourceQuestionIds { get; set; } = [];
}