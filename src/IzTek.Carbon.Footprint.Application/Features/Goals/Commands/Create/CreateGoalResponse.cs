namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Create;

public record CreateGoalResponse(
    Guid Id,
    int Month,
    int Year,
    int TargetTreeCount);