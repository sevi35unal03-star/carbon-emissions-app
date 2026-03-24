using AppCacheKeys = IzTek.Carbon.Footprint.Application.Common.Constants.CacheKeys;

namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.CreateGlobal;

public record CreateGlobalGoalCommand(
    int Month,
    int Year,
    int TargetTreeCount) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
    [
        AppCacheKeys.Goals.Detail(Month, Year),
        AppCacheKeys.Leaderboard.Monthly(Month, Year),
        AppCacheKeys.HomePage.Data(Month, Year)
    ];
}