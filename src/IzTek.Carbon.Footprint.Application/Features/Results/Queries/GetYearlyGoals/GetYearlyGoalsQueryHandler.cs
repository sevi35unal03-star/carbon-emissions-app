using IzTek.Carbon.Footprint.Application.Common.Extensions;

namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;

public static class GetYearlyGoalsQueryHandler
{
    public static async Task<Result<GetYearlyGoalsResponse>> Handle(
        GetYearlyGoalsQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ICacheService cache,
        CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var cacheKey = $"yearly-goals:{userId}:{query.Year}";

        // Cache check
        if (await cache.GetCachedResultAsync<GetYearlyGoalsResponse>(cacheKey, ct) is { } hit)
            return hit;

        var goals = await context.Goals
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Year == query.Year)
            .OrderBy(x => x.Month)
            .Select(x => new MonthlyGoalDto(
                x.Month,
                x.Year,
                x.TargetTreeCount,
                x.IsCompleted))
            .ToListAsync(ct);

        var yearlyTarget = goals.Sum(x => x.TargetTreeCount);

        var result = Result<GetYearlyGoalsResponse>.Success(
            new GetYearlyGoalsResponse(yearlyTarget, goals));

        // Cache set — 30 dakika, hedef güncellenince invalidate edilmeli
        await cache.SetCachedResultAsync(cacheKey, result, TimeSpan.FromMinutes(30), ct);

        return result;
    }
}