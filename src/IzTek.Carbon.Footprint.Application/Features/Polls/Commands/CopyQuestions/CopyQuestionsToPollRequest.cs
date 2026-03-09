namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.CopyQuestions;

public record CopyQuestionsToPollRequest(
    List<Guid> SourceQuestionIds,
    Guid PollSetId);