namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;

public static class GetMonthlyPollQueryHandler
{
    public static async Task<Result<GetMonthlyPollResponse>> Handle(
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var response = await context.PollSets
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetMonthlyPollResponse(
                PollSetId: x.Id,
                Name: x.Name,
                Description: x.Description,
                Questions: x.Questions
                    .OrderBy(q => q.DisplayOrder)
                    .Select(q => new PollQuestionResponse
                    {
                        Id = q.Id,
                        Text = q.Text,
                        DisplayOrder = q.DisplayOrder,
                        Options = q.Options
                            .OrderBy(o => o.DisplayOrder)
                            .Select(o => new PollOptionResponse
                            {
                                Id = o.Id,
                                Text = o.Text,
                                Message = o.Message,         // ← eklendi
                                CarbonValue = o.CarbonValue, // ← eklendi
                                NextPollQuestionId = o.NextPollQuestionId // ← eklendi
                            })
                            .ToList()
                    })
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct);

        if (response == null)
            return Result<GetMonthlyPollResponse>.Failure(
                SystemErrorCodes.ActivePollNotFound, HttpStatusCode.NotFound);

        return Result<GetMonthlyPollResponse>.Success(response);
    }
}