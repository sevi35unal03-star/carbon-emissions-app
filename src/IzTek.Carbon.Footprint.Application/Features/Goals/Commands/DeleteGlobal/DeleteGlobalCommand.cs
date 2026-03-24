using AppCacheKeys = IzTek.Carbon.Footprint.Application.Common.Constants.CacheKeys;

namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.DeleteGlobal;

public record DeleteGlobalGoalCommand(
    Guid Id,
    int Month,
    int Year) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
    [
        AppCacheKeys.Goals.Detail(Month, Year),
        AppCacheKeys.Goals.Yearly(Year),
        AppCacheKeys.HomePage.Data(Month, Year),
    ];
}