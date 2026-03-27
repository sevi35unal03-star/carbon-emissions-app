using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;

public static class SubmitActivityAnswerHandler
{
    public static async Task<Result<SubmitActivityAnswerResponse>> Handle(
        SubmitActivityAnswerCommand command,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;

        var option = await context.ActivityOptions
            .FirstOrDefaultAsync(o =>
                o.Id == command.SelectedOptionId &&
                o.ActivityQuestionId == command.QuestionId, ct);

        if (option is null)
            return Result<SubmitActivityAnswerResponse>.Failure(
                SystemErrorCodes.InvalidActivityOption, HttpStatusCode.BadRequest);

        var answer = new UserActivityAnswer(
            userId: userId,
            questionId: command.QuestionId,
            selectedOptionId: command.SelectedOptionId,
            carbonValue: option.CarbonValue,
            answeredAt: DateTime.UtcNow);
        context.UserActivityAnswers.Add(answer);

        var log = new UserActivityLog(
            userId: userId,
            questionId: command.QuestionId,
            optionId: command.SelectedOptionId,
            score: option.CarbonValue,
            selectedOptionId: command.SelectedOptionId,
            selectedOptionText: option.Text,
            carbonValue: option.CarbonValue);
        context.UserActivityLogs.Add(log);

        await context.SaveChangesAsync(ct);

        var today = DateTime.UtcNow.Date;
        var totalCarbon = await context.UserActivityLogs
            .Where(x => x.UserId == userId &&
                        x.ActivityDate >= today &&
                        x.ActivityDate < today.AddDays(1))
            .SumAsync(x => x.TotalCarbonScore, ct);

        if (option.NextQuestionId is null)
        {
            return Result<SubmitActivityAnswerResponse>.Success(new SubmitActivityAnswerResponse
            {
                NextQuestion = null,
                TotalCarbonScore = totalCarbon,
                IsFlowCompleted = true
            });
        }

        var nextQuestion = await context.ActivityQuestions
            .AsNoTracking()
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == option.NextQuestionId, ct);

        if (nextQuestion is null)
        {
            return Result<SubmitActivityAnswerResponse>.Success(new SubmitActivityAnswerResponse
            {
                NextQuestion = null,
                TotalCarbonScore = totalCarbon,
                IsFlowCompleted = true
            });
        }

        var now = DateTime.UtcNow;
        var endDateTime = nextQuestion.EndDate.Date.AddDays(1);
        var remainingSeconds = (long)Math.Max(0, (endDateTime - now).TotalSeconds);

        return Result<SubmitActivityAnswerResponse>.Success(new SubmitActivityAnswerResponse
        {
            NextQuestion = new DailyQuestionResponse(
                nextQuestion.Id,
                nextQuestion.Text,
                nextQuestion.DisplayOrder,
                nextQuestion.Options.Select(o => new DailyOptionResponse(
                    o.Id,
                    o.Text,
                    o.CarbonValue,
                    o.NextQuestionId
                )).ToList(),
                remainingSeconds),
            TotalCarbonScore = totalCarbon,
            IsFlowCompleted = false
        });
    }
}