namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Create;

public record CreateGoalCommand(
    int Month,
    int Year,
    int TargetTreeCount);