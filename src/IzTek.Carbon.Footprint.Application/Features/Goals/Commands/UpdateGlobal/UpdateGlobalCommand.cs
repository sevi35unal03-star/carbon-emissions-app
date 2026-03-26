namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.UpdateGlobal;

public record UpdateGlobalGoalCommand(
    int Month,
    int Year,
    int TargetTreeCount)
{
    public Guid Id { get; init; }
}