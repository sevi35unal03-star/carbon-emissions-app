using IzTek.Carbon.Footprint.Application.Common.Constants;

namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;

public record GetYearlyGoalsQuery(int Year) : ICacheableQuery
{
    public string CacheKey => CacheKeys.Goals.Yearly(Year);
    public TimeSpan? Expiry => TimeSpan.FromHours(6);
}