namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;

public record GetDailyQuestionsQuery();

public static class GetDailyQuestionsHandler
{
    public static async Task<Result<List<DailyQuestionResponse>>> Handle(
        GetDailyQuestionsQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var userId = currentUser.UserId;

        var answeredQuestionIds = await context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId
                     && x.ActivityDate >= today
                     && x.ActivityDate < today.AddDays(1))
            .Select(x => x.ActivityQuestionId)
            .Distinct()
            .ToListAsync(ct);

        var rootQuestions = await context.ActivityQuestions
            .AsNoTracking()
            .Include(q => q.Options)
                .ThenInclude(o => o.NextQuestion)
                    .ThenInclude(nq => nq!.Options)
                        .ThenInclude(o => o.NextQuestion)
                            .ThenInclude(nq => nq!.Options)
            .Where(q => q.IsActive)
            .Where(q => q.StartDate <= now && q.EndDate >= now)
            .Where(q => !context.ActivityOptions.Any(opt => opt.NextQuestionId == q.Id))
            .Where(q => !answeredQuestionIds.Contains(q.Id))
            .OrderBy(q => q.DisplayOrder)
            .Take(2) // ← max 50
            .ToListAsync(ct);

        var response = rootQuestions
            .Select(q => MapToResponse(q, now))
            .ToList();

        return Result<List<DailyQuestionResponse>>.Success(response);
    }

    // ✅ Recursive mapping — GetActivityQuestionByIdQueryHandler ile aynı pattern
    private static DailyQuestionResponse MapToResponse(ActivityQuestion question, DateTime now)
    {
        var endOfDay = question.EndDate.Date.AddDays(1);
        var remainingSeconds = (long)Math.Max(0, (endOfDay - now).TotalSeconds);

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
                        : null  // ✅ nested soru varsa recursive, yoksa null
                ))
                .ToList(),
            remainingSeconds
        );
    }
}