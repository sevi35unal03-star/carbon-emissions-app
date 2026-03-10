namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Create;

public record CreateGoalCommand(
    int Month,
    int Year,
    int TargetTreeCount) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
        [$"yearly-goals:{Year}",
         $"monthly-leaderboard:{Month}:{Year}"];
}