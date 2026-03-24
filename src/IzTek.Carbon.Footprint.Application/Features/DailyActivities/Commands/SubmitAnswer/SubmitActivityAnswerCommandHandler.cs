using IzTek.Carbon.Footprint.Application.Common.Extensions;
using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;

public static class SubmitActivityAnswerHandler
{
    public static async Task<Result<SubmitActivityAnswerResponse>> Handle(
        SubmitActivityAnswerCommand command,
        IApplicationDbContext context,
        ICacheService cache,
        CancellationToken ct)
    {
        // 1. Option doğrula
        var option = await context.ActivityOptions
            .FirstOrDefaultAsync(o =>
                o.Id == command.SelectedOptionId &&
                o.ActivityQuestionId == command.QuestionId, ct);

        if (option is null)
            return Result<SubmitActivityAnswerResponse>.Failure(
                SystemErrorCodes.InvalidActivityOption, HttpStatusCode.BadRequest);

        // 2. UserActivityAnswer
        var answer = new UserActivityAnswer(
            userId: command.UserId,
            questionId: command.QuestionId,
            selectedOptionId: command.SelectedOptionId,
            carbonValue: option.CarbonValue,
            answeredAt: DateTime.UtcNow);
        context.UserActivityAnswers.Add(answer);

        // 3. UserActivityLog
        var log = new UserActivityLog(
            userId: command.UserId,
            questionId: command.QuestionId,
            optionId: command.SelectedOptionId,
            score: option.CarbonValue,
            selectedOptionId: command.SelectedOptionId,
            selectedOptionText: option.Text,
            carbonValue: option.CarbonValue);
        context.UserActivityLogs.Add(log);

        await context.SaveChangesAsync(ct);

        // Cache invalidation
        await cache.InvalidateAsync(command, ct);

        // 4. Bugünkü toplam karbon hesapla
        var today = DateTime.UtcNow.Date;
        var totalCarbon = await context.UserActivityLogs
            .Where(x => x.UserId == command.UserId &&
                        x.ActivityDate >= today &&
                        x.ActivityDate < today.AddDays(1))
            .SumAsync(x => x.TotalCarbonScore, ct);

        // 5. Flow bitti mi?
        if (option.NextQuestionId is null)
        {
            return Result<SubmitActivityAnswerResponse>.Success(new SubmitActivityAnswerResponse
            {
                NextQuestion = null,
                TotalCarbonScore = totalCarbon,
                IsFlowCompleted = true
            });
        }

        // 6. NextQuestion getir
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

        // 7. NextQuestion ile devam et
        var now = DateTime.UtcNow;
        var endDateTime = nextQuestion.EndDate.Date.Add(nextQuestion.ScheduledTime);
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