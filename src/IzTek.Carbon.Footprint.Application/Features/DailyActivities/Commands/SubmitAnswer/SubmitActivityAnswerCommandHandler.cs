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

        // Flow bitti
        if (option.NextQuestionId is null)
        {
            return Result<SubmitActivityAnswerResponse>.Success(new SubmitActivityAnswerResponse
            {
                NextQuestion = null,
                TotalCarbonScore = totalCarbon,
                IsFlowCompleted = true
            });
        }

        // Sonraki soruyu nested tree olarak getir
        var nextQuestion = await context.ActivityQuestions
    // .AsNoTracking() ← kaldırın
    .Include(q => q.Options)
        .ThenInclude(o => o.NextQuestion)
            .ThenInclude(nq => nq!.Options)
                .ThenInclude(o => o.NextQuestion)
                    .ThenInclude(nq => nq!.Options)
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

        return Result<SubmitActivityAnswerResponse>.Success(new SubmitActivityAnswerResponse
        {
            NextQuestion = MapToResponse(nextQuestion),
            TotalCarbonScore = totalCarbon,
            IsFlowCompleted = false
        });
    }

    private static DailyQuestionResponse MapToResponse(ActivityQuestion question)
    {
        var now = DateTime.UtcNow;
        var endOfDay = question.EndDate.Date.AddDays(1);
        var remainingSeconds = (long)Math.Max(0, (endOfDay - now).TotalSeconds);

        return new DailyQuestionResponse(
            question.Id,
            question.Text,
            question.DisplayOrder,
            question.Options
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new DailyOptionResponse(
                    o.Id,
                    o.Text,
                    o.CarbonValue,
                    o.NextQuestionId,
                    o.NextQuestion is not null
                        ? MapToResponse(o.NextQuestion)
                        : null
                ))
                .ToList(),
            remainingSeconds
        );
    }
}