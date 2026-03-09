namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Update;

public record UpdatePollSetCommand(
    Guid PollSetId,
    string Name,
    int Month,
    int Year,
    string Description,
    int DisplayOrder
);