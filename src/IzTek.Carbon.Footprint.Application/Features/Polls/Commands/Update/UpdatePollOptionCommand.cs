namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Update;

public record UpdatePollOptionCommand(
    Guid OptionId,
    string Text,
    double Value,
    string? Message, // ← eklendi
    Guid? NextPollQuestionId,
    int DisplayOrder
);