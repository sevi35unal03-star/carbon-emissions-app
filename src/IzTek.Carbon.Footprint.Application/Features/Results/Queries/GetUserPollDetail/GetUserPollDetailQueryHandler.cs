namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

public static class GetUserPollDetailQueryHandler
{
    public static async Task<Result<UserPollDetailResponse>> HandleAsync(
        GetUserPollDetailQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Kullanıcının o aydaki ana sonucunu getir (Skor ve Ağaç sayısı için)
        var pollSummary = await context.UserPollResults
            .FirstOrDefaultAsync(x => x.UserId == query.UserId &&
                                     x.Month == query.Month &&
                                     x.Year == query.Year, ct);

        if (pollSummary == null)
            return Result.Failure<UserPollDetailResponse>("Bu aya ait anket kaydı bulunamadı.");

        // 2. Detaylı cevapları Soru ve Seçenek tablolarıyla birleştirerek getir
        var answers = await context.UserPollAnswers
            .Where(x => x.UserId == query.UserId && x.PollSetId == pollSummary.PollSetId)
            .Select(a => new PollAnswerDetailDto(
                a.Question.Text,       // PollQuestions tablosundan
                a.Option.Text,         // PollOptions tablosundan
                a.ScoreSnapshot        // Kayıt anındaki puanı
            ))
            .ToListAsync(ct);

        // 3. Sonucu birleştir
        var response = new UserPollDetailResponse(
            query.UserName,
            pollSummary.TotalScore,
            pollSummary.TreeCount,
            answers
        );

        return Result.Success(response);
    }
}