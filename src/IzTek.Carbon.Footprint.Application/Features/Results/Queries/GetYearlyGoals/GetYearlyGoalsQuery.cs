namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;

public record GetYearlyGoalsQuery(int Year) : ICacheableQuery
{
    public string CacheKey => $"yearly-goals:{Year}";
    public TimeSpan? Expiry => TimeSpan.FromHours(6);
}