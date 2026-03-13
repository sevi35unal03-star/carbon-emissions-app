using IzTek.Carbon.Footprint.Application.Common.Constants;

namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

public record GetMonthlyLeaderboardQuery(int Month, int Year) : ICacheableQuery
{
    public string CacheKey => CacheKeys.Leaderboard.Monthly(Month, Year);
    public TimeSpan? Expiry => TimeSpan.FromHours(1);
}