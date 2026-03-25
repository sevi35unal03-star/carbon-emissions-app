namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.CreateGlobal;

public record CreateGlobalGoalCommand(
    int Month,
    int Year,
    int TargetTreeCount);