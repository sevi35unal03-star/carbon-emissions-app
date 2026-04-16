namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;

public static class GetUserDailyResultsHandler
{
    public static async Task<Result<List<UserDailyResultResponse>>> Handle(
        GetUserDailyResultsQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // 1. Bugünkü aktivite loglarını çek
        var todayActivities = await context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.ActivityDate >= today && x.ActivityDate < tomorrow)
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                Count = g.Count(),
                CarbonScore = g.Sum(x => x.TotalCarbonScore)
            })
            .ToListAsync(ct);

        // 2. Kullanıcıların anket sonuçlarını çek
        var pollResults = await context.UserPollResults
            .AsNoTracking()
            .Where(x => x.IsCompleted)
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                LatestScore = g.OrderByDescending(x => x.CreatedAt)
                               .Select(x => x.TotalScore)
                               .FirstOrDefault()
            })
            .ToListAsync(ct);

        // 3. PointUnit'i çek
        var treeDefinition = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        var pointUnit = treeDefinition?.PointUnit ?? 500;

        // 4. Kullanıcıları çek
        var users = await context.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);

        // 5. In-memory join
        var results = users.Select(u =>
        {
            var activity = todayActivities.FirstOrDefault(a => a.UserId == u.Id);
            var pollResult = pollResults.FirstOrDefault(p => p.UserId == u.Id);

            return new UserDailyResultResponse
            {
                Id = u.Id,
                LastLoginDate = u.LastLoginDate,
                CarbonFootprintScore = pollResult?.LatestScore ?? 0,
                DailyActivitiesCount = activity?.Count ?? 0,
                TotalCurrentScore = u.TotalPoints,
                DonatedTreeCount = u.DonatedTreeCount,
                EquivalentPoints = u.DonatedTreeCount * pointUnit
            };
        }).ToList();

        return Result<List<UserDailyResultResponse>>.Success(results);
    }
}