namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.UpdateGlobal;

public record UpdateGlobalGoalCommand(
    Guid Id,
    int Month,
    int Year,
    int TargetTreeCount);