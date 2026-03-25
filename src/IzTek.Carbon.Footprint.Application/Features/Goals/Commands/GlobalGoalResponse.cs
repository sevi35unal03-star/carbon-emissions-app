namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands;

public record GlobalGoalResponse(Guid Id, int Month, int Year, int TargetTreeCount);