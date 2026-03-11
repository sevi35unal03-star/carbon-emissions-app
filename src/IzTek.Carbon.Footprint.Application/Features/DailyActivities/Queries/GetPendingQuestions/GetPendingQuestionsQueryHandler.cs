namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPendingQuestions;

public static class GetPendingQuestionsQueryHandler
{
    public static async Task<Result<PendingQuestionsResponse>> Handle(
        GetPendingQuestionsQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        var now = DateTime.UtcNow;
        var today = now.Date;

        // Bugün aktif olan tüm root soruları getir (GetDailyQuestionsHandler ile aynı kural)
        var activeQuestionIds = await context.ActivityQuestions
            .AsNoTracking()
            .Where(q => q.StartDate <= now && q.EndDate >= now)
            .Where(q => !context.ActivityOptions.Any(opt => opt.NextQuestionId == q.Id))
            .Select(q => q.Id)
            .ToListAsync(ct);

        if (!activeQuestionIds.Any())
            return Result<PendingQuestionsResponse>.Success(
                new PendingQuestionsResponse(HasPending: false, PendingCount: 0));

        // Kullanıcının bugün cevapladığı soruları getir
        var answeredQuestionIds = await context.UserActivityAnswers
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.AnsweredAt.Date == today)
            .Select(a => a.QuestionId)
            .ToListAsync(ct);

        var pendingCount = activeQuestionIds
            .Except(answeredQuestionIds)
            .Count();

        return Result<PendingQuestionsResponse>.Success(
            new PendingQuestionsResponse(
                HasPending: pendingCount > 0,
                PendingCount: pendingCount));
    }
}