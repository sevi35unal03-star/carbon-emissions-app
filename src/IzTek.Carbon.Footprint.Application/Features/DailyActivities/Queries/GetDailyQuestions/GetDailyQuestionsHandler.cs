namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;

public record GetDailyQuestionsQuery();

public static class GetDailyQuestionsHandler
{
    public static async Task<Result<List<DailyQuestionResponse>>> Handle(
    GetDailyQuestionsQuery query,
    IApplicationDbContext context,
    CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var rootQuestions = await context.ActivityQuestions
            .AsNoTracking()
            .Include(q => q.Options)
            .Where(q => q.StartDate <= now && q.EndDate >= now)
            .Where(q => q.IsActive)
            .Where(q => !context.ActivityOptions.Any(opt => opt.NextQuestionId == q.Id))
            .OrderBy(q => q.DisplayOrder)
            .Take(2)  // ← Maksimum 2 soru
            .ToListAsync(ct);

        var response = rootQuestions.Select(q =>
        {
            // Bitiş zamanı = EndDate'in günü + ScheduledTime
            var endDateTime = q.EndDate.Date.Add(q.ScheduledTime);
            var remainingSeconds = (long)Math.Max(0, (endDateTime - now).TotalSeconds);

            return new DailyQuestionResponse(
                q.Id,
                q.Text,
                q.DisplayOrder,
                q.Options.Select(o => new DailyOptionResponse(
                    o.Id,
                    o.Text,
                    o.CarbonValue,
                    o.NextQuestionId
                )).ToList(),
                remainingSeconds);  // ← "36 saat kaldı" = 129600 saniye
        }).ToList();

        return Result<List<DailyQuestionResponse>>.Success(response);
    }
}