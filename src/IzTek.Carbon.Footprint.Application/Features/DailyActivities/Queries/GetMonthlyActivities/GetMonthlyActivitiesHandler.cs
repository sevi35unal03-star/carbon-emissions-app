namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetMonthlyActivities;

public static class GetMonthlyActivitiesHandler
{
    public static async Task<Result<MonthlyActivityResponse>> Handle(
        GetMonthlyActivitiesQuery request,
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;

        var monthStart = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        DateTime periodStart = request.Period == 1
            ? monthStart
            : new DateTime(request.Year, request.Month, 16, 0, 0, 0, DateTimeKind.Utc);

        DateTime periodEnd = request.Period == 1
            ? new DateTime(request.Year, request.Month, 15, 23, 59, 59, DateTimeKind.Utc)
            : monthEnd;

        var allMonthLogs = await context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId &&
                        x.ActivityDate >= monthStart &&
                        x.ActivityDate <= monthEnd)
            .Select(x => new { x.ActivityDate, x.TotalCarbonScore })
            .ToListAsync(ct);

        // Bu periyotta hiç kayıt yoksa anlamlı mesaj dön
        var periodLogs = allMonthLogs
            .Where(x => x.ActivityDate >= periodStart && x.ActivityDate <= periodEnd)
            .ToList();

        if (!periodLogs.Any())
            return Result<MonthlyActivityResponse>.Failure(
                SystemErrorCodes.NoActivityFoundForPeriod,
                HttpStatusCode.NotFound);

        double totalMonthlyScore = allMonthLogs.Sum(x => x.TotalCarbonScore);

        var dailyScores = periodLogs
            .GroupBy(x => x.ActivityDate.Date)
            .Select(g => new DailyScoreDto(
                Date: g.Key,
                TotalScore: g.Sum(s => s.TotalCarbonScore)))
            .OrderBy(x => x.Date)
            .ToList();

        double totalPeriodScore = dailyScores.Sum(x => x.TotalScore);

        return Result<MonthlyActivityResponse>.Success(
            new MonthlyActivityResponse(totalMonthlyScore, totalPeriodScore, dailyScores));
    }
}