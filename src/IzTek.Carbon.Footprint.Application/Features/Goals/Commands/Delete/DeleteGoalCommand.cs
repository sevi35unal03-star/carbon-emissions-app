namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Delete;

public record DeleteGoalCommand(
    Guid Id,
    int Month,
    int Year);