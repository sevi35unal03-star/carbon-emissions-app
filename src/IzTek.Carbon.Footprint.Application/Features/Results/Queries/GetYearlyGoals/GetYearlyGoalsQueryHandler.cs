namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetYearlyGoals;

public class GetYearlyGoalsQueryHandler(IApplicationDbContext context)
{
    public async Task<Result<GetYearlyGoalsResponse>> HandleAsync(
    GetYearlyGoalsQuery query,
    ICurrentUserService currentUser,
    CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var goals = await context.Goals
            .Where(x => x.UserId == userId   // ← UserId filtresi
                     && x.Year == query.Year)
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