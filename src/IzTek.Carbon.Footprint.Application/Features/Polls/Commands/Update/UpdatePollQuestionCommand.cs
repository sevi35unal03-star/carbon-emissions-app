namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Update;

public record UpdatePollQuestionCommand(
    Guid QuestionId,
    string Text,
    int DisplayOrder
);