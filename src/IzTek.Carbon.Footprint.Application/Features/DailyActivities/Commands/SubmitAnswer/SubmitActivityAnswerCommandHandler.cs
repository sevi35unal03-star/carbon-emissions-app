using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.SubmitAnswer;


namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;

public class SubmitActivityAnswerHandler
{
    public async Task<Result<SubmitActivityAnswerResponse>> HandleAsync(
        SubmitActivityAnswerCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Option doğrula
        var option = await context.ActivityOptions
            .FirstOrDefaultAsync(o =>
                o.PollQuestionId == command.SelectedOptionId &&
                o.ActivityQuestionId == command.QuestionId,
                ct);

        if (option is null)
            return Result<SubmitActivityAnswerResponse>.Failure(
                SystemErrorCodes.InvalidActivityOption, HttpStatusCode.BadRequest);

        // 2. Cevabı kaydet
        var log = new UserActivityLog(
            command.UserId,
            command.QuestionId,
            command.SelectedOptionId,
            option.CarbonValue);

        context.UserActivityLogs.Add(log);
        await context.SaveChangesAsync(ct);

        // 3. Bugünkü toplam karbon hesapla
        var today = DateTime.UtcNow.Date;
        var totalCarbon = await context.UserActivityLogs
            .Where(x => x.UserId == command.UserId &&
                        x.ActivityDate >= today &&
                        x.ActivityDate < today.AddDays(1))
            .SumAsync(x => x.TotalCarbonScore, ct);

        // 4. Flow bitti mi?
        if (option.NextQuestionId is null)
        {
            return Result<SubmitActivityAnswerResponse>.Success(new SubmitActivityAnswerResponse
            {
                NextQuestion = null,
                TotalCarbonScore = totalCarbon,
                IsFlowCompleted = true
            });
        }

        // 5. NextQuestion getir
        var nextQuestion = await context.ActivityQuestions
            .AsNoTracking()
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.PollQuestionId == option.NextQuestionId, ct);

        if (nextQuestion is null)
        {
            return Result<SubmitActivityAnswerResponse>.Success(new SubmitActivityAnswerResponse
            {
                NextQuestion = null,
                TotalCarbonScore = totalCarbon,
                IsFlowCompleted = true
            });
        }

        // 6. NextQuestion ile devam et
        return Result<SubmitActivityAnswerResponse>.Success(new SubmitActivityAnswerResponse
        {
            NextQuestion = new DailyQuestionResponse(
                nextQuestion.PollQuestionId,
                nextQuestion.Text,
                nextQuestion.DisplayOrder,
                nextQuestion.Options.Select(o => new DailyOptionResponse(
                    o.PollQuestionId,
                    o.Text,
                    o.CarbonValue,
                    o.NextQuestionId
                )).ToList()
            ),
            TotalCarbonScore = totalCarbon,
            IsFlowCompleted = false
        });
    }
}