namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.CopyQuestions;

public record CopyQuestionsToPollCommand(
    Guid PollSetId,
    List<Guid> SourceQuestionIds);