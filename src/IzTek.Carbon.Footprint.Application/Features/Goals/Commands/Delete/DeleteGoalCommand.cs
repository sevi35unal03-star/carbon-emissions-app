namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Delete;

public record DeleteGoalCommand(
    Guid Id,
    int Month,
    int Year) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
        [$"yearly-goals:{Year}",
         $"monthly-leaderboard:{Month}:{Year}"];
}