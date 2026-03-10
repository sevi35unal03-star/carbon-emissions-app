namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Update;

public record UpdateGoalCommand(
    Guid Id,
    int Month,
    int Year,
    int TargetTreeCount) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
        [$"yearly-goals:{Year}",
         $"monthly-leaderboard:{Month}:{Year}"];
}