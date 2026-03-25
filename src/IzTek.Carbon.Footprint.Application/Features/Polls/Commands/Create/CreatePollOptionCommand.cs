namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public record CreatePollOptionCommand(
    Guid QuestionId,
    string Text,
    double Value,
    string? Message, // ← eklendi
    int DisplayOrder
);