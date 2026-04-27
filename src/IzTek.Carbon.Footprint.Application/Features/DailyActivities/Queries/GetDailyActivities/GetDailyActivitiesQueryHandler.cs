using System.Globalization;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyActivities;

public class GetDailyActivitiesQueryHandler
{
    public async Task<Result<DailyActivityResponse>> Handle(
        GetDailyActivitiesQuery query,
        IApplicationDbContext context,
        ICurrentUserService userService,
        CancellationToken ct)
    {
        var userId = userService.UserId;

        // 1. Paralel sorgular
        var questionsTask = context.ActivityQuestions
            .AsNoTracking()
            .Include(x => x.Options)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(ct);

        var userLogsTask = context.UserActivityLogs
            .AsNoTracking()
            .Include(x => x.ActivityQuestion)
            .Include(x => x.ActivityOption) // ✅ Option text için include
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

        await Task.WhenAll(questionsTask, userLogsTask);

        var questions = await questionsTask;
        var userLogs = await userLogsTask;

        // 2. Geçmiş — güne göre grupla
        var history = userLogs
            .GroupBy(l => l.CreatedAt.Date)
            .Select(group => new HistoryGroupDto(
                group.Key.ToString("D", new CultureInfo("tr-TR")),
                [.. group.Select(x => new CompletedActivityDto(
                    x.ActivityQuestion.Text,
                    x.ActivityOption.Text,
                    x.TotalCarbonScore,
                    x.CreatedAt))]
            )).ToList();

        // 3. Bekleyen sorular
        var answeredIds = userLogs
            .Select(l => l.ActivityQuestionId)
            .ToHashSet();

        var pending = questions
            .Where(q => !answeredIds.Contains(q.Id))
            .Select(q => new PendingQuestionDto(
                q.Id,
                q.Text,
                q.ScheduledTime,
                [.. q.Options.Select(o => new OptionDto(
                    o.Id,
                    o.Text,
                    o.NextQuestionId,
                    o.NextQuestionId is null // ✅ NextQuestionId yoksa final step
                ))]
            )).ToList();

        var response = new DailyActivityResponse(pending, history);

        return Result<DailyActivityResponse>.Success(response);
    }
}


/* [summary ] 
 Kullanıcının

Cevaplamadığı soruları (Pending)

Daha önce cevapladığı aktiviteleri (History)

tek response içinde döndürmek.

Kullanıcının aktiviteleri tarihe göre gruplanıyor.

Her gün için:

Soru metni

Seçilen seçenek

Kazanılan puan

Tarih

oluşturuluyor.


.Where(q => !userLogs.Any(log => log.ActivityQuestionId == q.Id)) yerine

Daha doğru yöntem:

var answeredIds = userLogs.Select(x => x.ActivityQuestionId).ToHashSet();

.Where(q => !answeredIds.Contains(q.Id))

Performans ciddi artar.
 */