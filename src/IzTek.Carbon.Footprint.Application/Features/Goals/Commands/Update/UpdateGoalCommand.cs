using AppCacheKeys = IzTek.Carbon.Footprint.Application.Common.Constants.CacheKeys;

namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.Update;

public record UpdateGoalCommand(
    Guid Id,
    int Month,
    int Year,
    int TargetTreeCount) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
    [
        AppCacheKeys.Goals.Yearly(Year),
        AppCacheKeys.Leaderboard.Monthly(Month, Year),
        AppCacheKeys.HomePage.Data(Month, Year)
    ];
}