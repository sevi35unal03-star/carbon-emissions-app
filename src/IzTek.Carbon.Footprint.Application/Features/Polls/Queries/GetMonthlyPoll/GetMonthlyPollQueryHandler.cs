namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;

public static class GetMonthlyPollQueryHandler
{
    public static async Task<Result<GetMonthlyPollResponse>> Handle(
    GetMonthlyPollQuery query,
    IApplicationDbContext context,
    ICurrentUserService currentUser, // ← ekle
    CancellationToken ct)
    {
        var pollSet = await context.PollSets
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
                                Message = o.Message,
                                CarbonValue = o.CarbonValue,
                                NextPollQuestionId = o.NextPollQuestionId
                            })
                            .ToList()
                    })
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct);

        if (pollSet == null)
            return Result<GetMonthlyPollResponse>.Failure(
                SystemErrorCodes.ActivePollNotFound, HttpStatusCode.NotFound);

        // Kullanıcının taslağı var mı kontrol et
        var draft = await context.UserPollResults
            .AsNoTracking()
            .Include(x => x.Answers)
            .FirstOrDefaultAsync(x => x.UserId == currentUser.UserId
                                   && x.PollSetId == pollSet.PollSetId
                                   && !x.IsCompleted, ct);

        // Taslak varsa seçili cevapları işaretle
        if (draft is not null)
        {
            foreach (var question in pollSet.Questions)
            {
                var draftAnswer = draft.Answers
                    .FirstOrDefault(a => a.PollQuestionId == question.Id);

                if (draftAnswer is not null)
                    question.SelectedOptionId = draftAnswer.PollOptionId;
            }
        }

        return Result<GetMonthlyPollResponse>.Success(pollSet);
    }
}