namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetList;

public static class GetActivityQuestionsQueryHandler
{
    public static async Task<Result<List<ActivityQuestionResponse>>> Handle(
        GetActivityQuestionsQuery request,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var result = await context.ActivityQuestions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .Include(x => x.Options)
            .Select(x => new ActivityQuestionResponse
            {
                Id = x.Id,
                Text = x.Text,
                DisplayOrder = x.DisplayOrder,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                ScheduledTime = x.ScheduledTime,
                Options = x.Options.Select(o => new ActivityOptionResponse
                {
                    Id = o.Id,
                    Text = o.Text,
                    CarbonValue = o.CarbonValue,
                    NextQuestionId = o.NextQuestionId
                }).ToList()
            })
            .ToListAsync(ct);

        return Result<List<ActivityQuestionResponse>>.Success(result);
    }
}