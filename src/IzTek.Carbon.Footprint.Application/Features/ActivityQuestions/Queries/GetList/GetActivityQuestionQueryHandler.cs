namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetList;

public static class GetActivityQuestionsQueryHandler
{
    public static async Task<Result<List<ActivityQuestionResponse>>> Handle(
        GetActivityQuestionsQuery request,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var query = context.ActivityQuestions
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder);

        var result = await query
            .ProjectToType<ActivityQuestionResponse>()
            .ToListAsync(ct);

        return Result<List<ActivityQuestionResponse>>.Success(result);
    }
}