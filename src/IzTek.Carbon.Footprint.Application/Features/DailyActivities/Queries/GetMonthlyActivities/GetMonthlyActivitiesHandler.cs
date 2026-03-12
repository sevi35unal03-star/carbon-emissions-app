namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetMonthlyActivities;

public static class GetMonthlyActivitiesHandler
{
public static async Task<Result<MonthlyActivityResponse>> Handle(
    ICurrentUserService currentUserService,
    IApplicationDbContext context,
    GetMonthlyActivitiesQuery request,
    CancellationToken ct)
    {
        var userId = currentUserService.UserId;

        // 1. Tüm ayın sınırlarını belirle (Üstteki toplam kartı için)
        var monthStart = new DateTime(request.Year, request.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        // 2. Seçili dönemin (pagination) sınırlarını belirle
        DateTime periodStart = request.Period == 1 ? monthStart : new DateTime(request.Year, request.Month, 16);
        DateTime periodEnd = request.Period == 1 ? new DateTime(request.Year, request.Month, 15) : monthEnd;

        // 3. Veritabanından tüm ayın loglarını çek (Tek sorguda halletmek için)
        var allMonthLogs = await context.UserActivityLogs
            .Where(x => x.UserId == userId &&
                        x.ActivityDate >= monthStart &&
                        x.ActivityDate <= monthEnd)
            .Select(x => new { x.ActivityDate, x.TotalCarbonScore })
            .ToListAsync(ct);

        // 4. Ayın genel toplamını hesapla (Tasarımın en üstündeki değer)
        double totalMonthlyScore = allMonthLogs.Sum(x => x.TotalCarbonScore);

        // 5. Sadece seçili döneme (Period) ait olanları filtrele ve grupla
        var dailyScores = allMonthLogs
            .Where(x => x.ActivityDate >= periodStart && x.ActivityDate <= periodEnd)
            .GroupBy(x => x.ActivityDate.Date)
            .Select(g => new DailyScoreDto(
                Date: g.Key,
                TotalScore: g.Sum(s => s.TotalCarbonScore)
            ))
            .OrderBy(x => x.Date)
            .ToList();

        // 6. Dönem toplamını hesapla
        double totalPeriodScore = dailyScores.Sum(x => x.TotalScore);

        return Result<MonthlyActivityResponse>.Success(
            new MonthlyActivityResponse(totalMonthlyScore, totalPeriodScore, dailyScores));

        }

    }


/* [summary]
 Seçilen ay için:

Ayın toplam puanı

Seçilen yarı dönemin puanı (1-15 / 16-30)

Günlük skor listesi

döndürmek.
 */