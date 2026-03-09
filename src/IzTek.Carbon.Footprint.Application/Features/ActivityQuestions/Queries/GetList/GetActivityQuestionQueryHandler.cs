namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetList;

public static class GetActivityQuestionsQueryHandler
{
    public static async Task<List<ActivityQuestionResponse>> HandleAsync(
        GetActivityQuestionsQuery request,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        return await context.ActivityQuestions
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ProjectToType<ActivityQuestionResponse>()
            .ToListAsync(ct);
    }
}