namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.DeleteGlobal;

public record DeleteGlobalGoalCommand(
    Guid Id,
    int Month,
    int Year);