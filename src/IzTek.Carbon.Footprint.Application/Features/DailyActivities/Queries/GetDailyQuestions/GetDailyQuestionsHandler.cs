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
            .Where(q => !context.ActivityOptions.Any(opt => opt.NextQuestionId == q.Id)) // ✅ PollQuestionId → Id
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync(ct);

        var response = rootQuestions.Select(q => new DailyQuestionResponse(
            q.Id,
            q.Text,
            q.DisplayOrder,
            q.Options.Select(o => new DailyOptionResponse(
                o.Id,
                o.Text,
                o.CarbonValue,
                o.NextQuestionId
            )).ToList()
        )).ToList();

        return Result<List<DailyQuestionResponse>>.Success(response);
    }
}