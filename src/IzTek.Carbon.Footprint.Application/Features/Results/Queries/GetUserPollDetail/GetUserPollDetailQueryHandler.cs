namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

public static class GetUserPollDetailQueryHandler
{
    public static async Task<Result<UserPollDetailResponse>> Handle(
        GetUserPollDetailQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        CancellationToken ct)
    {
        // User/Admin ayrımı:
        // Normal kullanıcı → token'dan userId alınır, TargetUserId gönderemez
        // Admin            → TargetUserId varsa onu kullanır, yoksa kendi token'ından alır
        var isAdmin = currentUserService.IsInRole("Admin");
        var resolvedUserId = (isAdmin && query.TargetUserId.HasValue)
            ? query.TargetUserId.Value
            : currentUserService.UserId!.Value;

        // Kullanıcının o aydaki anket özetini cevaplarıyla birlikte getir
        var pollResult = await context.UserPollResults
            .AsNoTracking()
            .Include(x => x.Answers)
            .FirstOrDefaultAsync(x => x.UserId == resolvedUserId &&
                                      x.Month == query.Month &&
                                      x.Year == query.Year, ct);

        if (pollResult is null)
            return Result<UserPollDetailResponse>.Failure(
                SystemErrorCodes.PollResultNotFound, HttpStatusCode.NotFound);

        var answers = pollResult.Answers
            .Select(x => new PollAnswerDetailDto
            {
                QuestionText = x.QuestionText,
                SelectedOptionText = x.SelectedOptionText,
                CarbonValue = x.CarbonValue
            })
            .ToList();

        var response = new UserPollDetailResponse(
            UserName: $"{pollResult.Name} {pollResult.Surname}",
            TotalScore: pollResult.TotalScore,
            TreeCount: pollResult.TreeCount,
            Answers: answers);

        return Result<UserPollDetailResponse>.Success(response);
    }
}