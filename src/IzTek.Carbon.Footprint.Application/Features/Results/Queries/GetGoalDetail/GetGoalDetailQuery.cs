namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetGoalDetail;

public record GetGoalDetailQuery(int Month, int Year) : ICacheableQuery
{
    public string CacheKey => $"goal-detail:{Month}:{Year}";
    public TimeSpan? Expiry => TimeSpan.FromHours(12);
}
