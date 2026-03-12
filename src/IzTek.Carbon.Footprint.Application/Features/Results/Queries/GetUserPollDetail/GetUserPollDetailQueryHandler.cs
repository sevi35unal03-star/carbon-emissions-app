namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

public static class GetUserPollDetailQueryHandler
{
    public static async Task<Result<UserPollDetailResponse>> Handle(
        GetUserPollDetailQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        CancellationToken ct)
    {
        // User/Admin ayirimi:
        // Normal kullanici -> JWT'den userId alinir, TargetUserId gonderse bile dikkate alinmaz
        // Admin            -> TargetUserId varsa o kullanicinin sonucu, yoksa kendi sonucu
        var isAdmin = currentUserService.IsInRole("Admin");
        var resolvedUserId = (isAdmin && query.TargetUserId.HasValue)
            ? query.TargetUserId.Value
            : currentUserService.UserId!.Value;

        // PollSetId + UserId + Month + Year kombinasyonu unique olmali
        var pollResult = await context.UserPollResults
            .AsNoTracking()
            .Include(x => x.Answers)
            .FirstOrDefaultAsync(x =>
                x.PollSetId == query.PollSetId &&
                x.UserId == resolvedUserId &&
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

        return Result<UserPollDetailResponse>.Success(new UserPollDetailResponse(
            UserName: $"{pollResult.Name} {pollResult.Surname}",
            TotalScore: pollResult.TotalScore,
            TreeCount: pollResult.TreeCount,
            Answers: answers));
    }
}