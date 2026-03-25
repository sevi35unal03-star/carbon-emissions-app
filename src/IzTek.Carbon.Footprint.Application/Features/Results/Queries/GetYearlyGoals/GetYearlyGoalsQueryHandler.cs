namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;

public static class GetYearlyGoalsQueryHandler
{
    public static async Task<Result<GetYearlyGoalsResponse>> Handle(
        GetYearlyGoalsQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId;

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

        return Result<GetYearlyGoalsResponse>.Success(
            new GetYearlyGoalsResponse(yearlyTarget, goals));
    }
}