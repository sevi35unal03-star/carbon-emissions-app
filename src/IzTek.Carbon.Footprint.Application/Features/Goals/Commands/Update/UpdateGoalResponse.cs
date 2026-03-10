namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Update;

public record UpdateGoalResponse(
    Guid Id,
    int Month,
    int Year,
    int TargetTreeCount);