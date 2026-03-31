namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public static class SubmitPollAnswerCommandHandler
{
    public static async Task<Result<SubmitPollAnswerResponse>> Handle(
    SubmitPollAnswerCommand command,
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    CancellationToken ct)
    {
        var optionIds = command.Answers.Select(a => a.OptionId).ToList();

        var options = await context.PollOptions
            .Include(o => o.PollQuestion)
            .Where(o => optionIds.Contains(o.Id))
            .ToListAsync(ct);

        if (!options.Any())
            return Result<SubmitPollAnswerResponse>.Failure(
                SystemErrorCodes.InvalidPollAnswers, HttpStatusCode.BadRequest);

        var treeDef = await context.TreeDefinitions
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        double totalCarbonScore = options.Sum(x => x.CarbonValue);

        int calculatedTrees = treeDef is not null && treeDef.PointUnit > 0
            ? (int)((totalCarbonScore / treeDef.PointUnit) * treeDef.TreeCount)
            : 0;

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == currentUser.UserId, ct);

        if (user is null)
            return Result<SubmitPollAnswerResponse>.Failure(
                SystemErrorCodes.UserNotFound, HttpStatusCode.NotFound);

        var answerList = command.Answers
            .Select(a =>
            {
                var option = options.FirstOrDefault(o => o.Id == a.OptionId);
                return (
                    questionId: a.QuestionId,
                    optionId: a.OptionId,
                    questionText: option?.PollQuestion?.Text ?? string.Empty,
                    optionText: option?.Text ?? string.Empty,
                    carbonValue: option?.CarbonValue ?? 0
                );
            })
            .ToList();

        // Taslak var mı kontrol et
        var draft = await context.UserPollResults
            .Include(x => x.Answers)
            .FirstOrDefaultAsync(x => x.UserId == currentUser.UserId
                                   && x.PollSetId == command.PollSetId
                                   && !x.IsCompleted, ct);

        if (draft is not null)
        {
            draft.UpdateDraft(totalCarbonScore, calculatedTrees, answerList);
            if (!command.IsDraft)
                draft.Complete();
        }
        else
        {
            var pollResult = new UserPollResult(
                name: user.Name ?? string.Empty,
                surname: user.Surname ?? string.Empty,
                userId: currentUser.UserId!.Value,
                pollSetId: command.PollSetId,
                totalScore: totalCarbonScore,
                treeCount: calculatedTrees);

            context.UserPollResults.Add(pollResult);

            foreach (var a in answerList)
                pollResult.AddAnswer(a.questionId, a.optionId, a.questionText, a.optionText, a.carbonValue);

            if (!command.IsDraft)
                pollResult.Complete();
        }

        if (!command.IsDraft)
            user.UpdateMonthlyCarbonResult(totalCarbonScore);

        await context.SaveChangesAsync(ct);

        return Result<SubmitPollAnswerResponse>.Success(new SubmitPollAnswerResponse(
            totalCarbonScore,
            calculatedTrees));
    }
}