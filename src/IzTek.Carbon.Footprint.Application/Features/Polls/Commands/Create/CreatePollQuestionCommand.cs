namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public record CreatePollQuestionCommand(
    Guid PollSetId,
    string Text,
    int DisplayOrder
);