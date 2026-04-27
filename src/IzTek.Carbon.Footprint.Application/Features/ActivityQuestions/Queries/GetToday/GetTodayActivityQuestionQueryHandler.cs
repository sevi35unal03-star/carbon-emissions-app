namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetToday;

public static class GetTodayActivityQuestionQueryHandler
{
    public static async Task<Result<ActivityQuestionResponse>> Handle(
        GetTodayActivityQuestionQuery request,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var today = DateTime.UtcNow;

        var allQuestions = await context.ActivityQuestions
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted
                     && x.StartDate <= today
                     && x.EndDate >= today)
            .Include(x => x.Options)
                .ThenInclude(o => o.NextQuestion)
                    .ThenInclude(nq => nq!.Options)
                        .ThenInclude(o => o.NextQuestion)
                            .ThenInclude(nq => nq!.Options)
            .ToListAsync(ct);

        if (!allQuestions.Any())
            return Result<ActivityQuestionResponse>.Failure(
                SystemErrorCodes.ActivityQuestionNotFound,
                HttpStatusCode.NotFound);

        var referencedIds = allQuestions
            .SelectMany(q => q.Options)
            .Where(o => o.NextQuestionId.HasValue)
            .Select(o => o.NextQuestionId!.Value)
            .ToHashSet();

        var rootQuestion = allQuestions
            .Where(q => !referencedIds.Contains(q.Id))
            .OrderBy(q => q.DisplayOrder)
            .FirstOrDefault();

        if (rootQuestion is null)
            return Result<ActivityQuestionResponse>.Failure(
                SystemErrorCodes.ActivityQuestionNotFound,
                HttpStatusCode.NotFound);

        return Result<ActivityQuestionResponse>.Success(MapToResponse(rootQuestion));
    }

    private static ActivityQuestionResponse MapToResponse(ActivityQuestion question)
    {
        return new ActivityQuestionResponse
        {
            Id = question.Id,
            Text = question.Text,
            DisplayOrder = question.DisplayOrder,
            StartDate = question.StartDate,
            EndDate = question.EndDate,
            ScheduledTime = question.ScheduledTime, // ← NotificationTime → ScheduledTime
            Options = question.Options
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new ActivityOptionResponse
                {
                    Id = o.Id,
                    Text = o.Text,
                    CarbonValue = o.CarbonValue,
                    NextQuestionId = o.NextQuestionId,
                    NextQuestion = o.NextQuestion is not null
                        ? MapToResponse(o.NextQuestion)
                        : null
                })
                .ToList()
        };
    }
}