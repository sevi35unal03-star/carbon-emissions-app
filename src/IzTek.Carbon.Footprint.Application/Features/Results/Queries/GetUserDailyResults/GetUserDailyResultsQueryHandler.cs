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

        // 2. Kullanıcıları çek
        var users = await context.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);

        // 3. In-memory join — nullable object hatası önlenir
        var results = users.Select(u =>
        {
            var activity = todayActivities.FirstOrDefault(a => a.UserId == u.Id);

            return new UserDailyResultResponse
            {
                Id = u.Id,
                LastLoginDate = u.LastLoginDate,
                CarbonFootprintScore = activity?.CarbonScore ?? 0,
                DailyActivitiesCount = activity?.Count ?? 0,
                TotalCurrentScore = u.TotalPoints,
                DonatedTreeCount = u.DonatedTreeCount,
                EquivalentPoints = u.TotalPoints
            };
        }).ToList();

        return Result<List<UserDailyResultResponse>>.Success(results);
    }
}