namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

public static class GetUserPollDetailQueryHandler
{

    public static async Task<Result<UserPollDetailResponse>> HandleAsync(
    GetUserPollDetailQuery query,
    IApplicationDbContext context,
    CancellationToken ct)
    {
        // 1. Kullanıcının o aydaki ana sonucunu getir
        var pollSummary = await context.UserPollResults
            .FirstOrDefaultAsync(x => x.UserId == query.UserId &&
                                     x.Month == query.Month &&
                                     x.Year == query.Year, ct);

        if (pollSummary == null)
            return Result<UserPollDetailResponse>.Failure(
                SystemErrorCodes.PollResultNotFound, HttpStatusCode.NotFound);

        // 2. UserAnswer üzerinden soru ve seçenek bilgilerini join ile getir
        var answers = await context.UserActivityAnswers
            .Where(x => x.UserId == query.UserId)
            .Join(context.PollQuestions,
                answer => answer.QuestionId,
                question => question.Id,
                (answer, question) => new { answer, question })
            .Join(context.PollOptions,
                combined => combined.answer.SelectedOptionId,
                option => option.Id,
                (combined, option) => new PollAnswerDetailDto
                {
                    QuestionText = combined.question.Text,
                    SelectedOptionText = option.Text,
                    CarbonValue = option.CarbonValue
                })
            .ToListAsync(ct);

        // 3. Sonucu birleştir
        var response = new UserPollDetailResponse(
            query.UserName,
            pollSummary.TotalScore,
            pollSummary.TreeCount,
            answers);

        return Result<UserPollDetailResponse>.Success(response);
    }
}