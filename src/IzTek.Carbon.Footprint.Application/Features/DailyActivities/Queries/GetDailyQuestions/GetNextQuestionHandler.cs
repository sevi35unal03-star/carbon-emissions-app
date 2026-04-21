namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;

public record GetNextQuestionQuery(
    Guid QuestionId,
    Guid SelectedOptionId,
    Guid UserId
);

public static class GetNextQuestionHandler
{
    public static async Task<DailyQuestionResponse?> Handle(
        GetNextQuestionQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var question = await context.ActivityQuestions
            .AsNoTracking()
            .Include(q => q.Options)
                .ThenInclude(o => o.NextQuestion)
                    .ThenInclude(nq => nq!.Options)
                        .ThenInclude(o => o.NextQuestion)
                            .ThenInclude(nq => nq!.Options)
            .FirstOrDefaultAsync(q => q.Id == query.QuestionId, ct);

        if (question == null) return null;

        var now = DateTime.UtcNow;
        return MapToResponse(question, now);
    }

    private static DailyQuestionResponse MapToResponse(ActivityQuestion question, DateTime now)
    {
        var endDateTime = question.EndDate.Date.AddDays(1);
        var remainingSeconds = (long)Math.Max(0, (endDateTime - now).TotalSeconds);

        return new DailyQuestionResponse(
            question.Id,
            question.Text,
            question.DisplayOrder,
            question.Options
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new DailyOptionResponse(
                    o.Id,
                    o.Text,
                    o.CarbonValue,
                    o.NextQuestionId,
                    o.NextQuestion is not null
                        ? MapToResponse(o.NextQuestion, now)
                        : null
                ))
                .ToList(),
            remainingSeconds
        );
    }
}