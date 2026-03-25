namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public static class SubmitPollAnswerCommandHandler
{
    public static async Task<Result<SubmitPollAnswerResponse>> Handle(
        SubmitPollAnswerCommand command,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var optionIds = command.Answers
            .Select(a => a.OptionId)
            .ToList();

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

        var pollResult = new UserPollResult(
            name: user.Name,
            surname: user.Surname,
            userId: currentUser.UserId!.Value,
            pollSetId: command.PollSetId,
            totalScore: totalCarbonScore,
            treeCount: calculatedTrees);

        context.UserPollResults.Add(pollResult);

        foreach (var answer in command.Answers)
        {
            var option = options.FirstOrDefault(o => o.Id == answer.OptionId);
            if (option is null) continue;

            pollResult.AddAnswer(
                pollQuestionId: answer.QuestionId,
                pollOptionId: answer.OptionId,
                questionText: option.PollQuestion?.Text ?? string.Empty,
                selectedOptionText: option.Text,
                carbonValue: option.CarbonValue);
        }

        user.UpdateMonthlyCarbonResult(totalCarbonScore);
        await context.SaveChangesAsync(ct);

        return Result<SubmitPollAnswerResponse>.Success(new SubmitPollAnswerResponse(
            totalCarbonScore,
            calculatedTrees));
    }
}