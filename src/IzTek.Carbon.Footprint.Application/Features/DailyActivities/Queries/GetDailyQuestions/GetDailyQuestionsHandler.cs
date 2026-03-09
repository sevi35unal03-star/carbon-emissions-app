namespace Iztek.Carbon.Footprint.Application.Features.DailyActivites.Queries.GetDailyQuestions;
public record GetDailyQuestionsQuery();

public static class GetDailyQuestionsHandler
{
    public static async Task<List<DailyQuestionResponse>> Handle(
        GetDailyQuestionsQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // KURAL: Sadece hiçbir seçeneğe bağlı olmayan (Parent'ı olmayan) soruları getir.
        var rootQuestions = await context.ActivityQuestions
            .AsNoTracking()
            .Include(q => q.Options)
            .Where(q => q.StartDate <= now && q.EndDate >= now)
            .Where(q => !context.ActivityOptions.Any(opt => opt.NextQuestionId == q.PollQuestionId))
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync(ct);

        return rootQuestions.Select(q => new DailyQuestionResponse(
            q.PollQuestionId,
            q.Text,
            q.DisplayOrder,
            q.Options.Select(o => new DailyOptionResponse(
                o.PollQuestionId,
                o.Text,
                o.CarbonValue,
                o.NextQuestionId // Eğer null değilse, frontend bunu kullanarak bir sonraki soruyu isteyecek
            )).ToList()
        )).ToList();
    }
}