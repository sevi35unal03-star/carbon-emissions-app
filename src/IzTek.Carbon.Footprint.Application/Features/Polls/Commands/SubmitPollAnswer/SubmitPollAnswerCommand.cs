namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public record PollAnswerItem(Guid QuestionId, Guid OptionId);

public record SubmitPollAnswerCommand(
    Guid PollSetId,
    List<PollAnswerItem> Answers,
    bool IsDraft = false); // ← yeni

