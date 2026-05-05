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

        // 1. Ayın Başlangıç ve Bitişini Hesapla
        var monthStart = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

        // 2. Veritabanından Veriyi Tek Seferde Çek
        var logs = await context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId &&
                        x.ActivityDate >= monthStart &&
                        x.ActivityDate <= monthEnd)
            .Select(x => new { x.ActivityDate, x.TotalCarbonScore })
            .ToListAsync(ct);

        // 3. Veri Yoksa Hızlıca Dön
        if (!logs.Any())
            return Result<MonthlyActivityResponse>.Failure(
                SystemErrorCodes.NoActivityFoundForPeriod,
                HttpStatusCode.NotFound);

        // 4. Günlük Skorları Grupla ve Modelle
        var dailyScores = logs
            .GroupBy(x => x.ActivityDate.Date)
            .Select(g => new DailyScoreDto(
                Date: g.Key,
                TotalScore: g.Sum(s => s.TotalCarbonScore)))
            .OrderBy(x => x.Date)
            .ToList();

        // 5. Aylık Toplam Skoru Hesapla
        double totalMonthlyScore = dailyScores.Sum(x => x.TotalScore);

        return Result<MonthlyActivityResponse>.Success(
            new MonthlyActivityResponse(totalMonthlyScore, dailyScores));
    }
}