using AppCacheKeys = IzTek.Carbon.Footprint.Application.Common.Constants.CacheKeys;

namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.UpdateGlobal;

public record UpdateGlobalGoalCommand(
    Guid Id,
    int Month,
    int Year,
    int TargetTreeCount) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
    [
        AppCacheKeys.Goals.Detail(Month, Year),
        AppCacheKeys.Goals.Yearly(Year),
        AppCacheKeys.HomePage.Data(Month, Year),
    ];
}