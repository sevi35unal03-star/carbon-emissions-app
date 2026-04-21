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
        var now = DateTime.UtcNow;
        var today = now.Date;

        // Gönderilen tüm option'ları tek seferde getir
        var optionIds = command.Answers.Select(a => a.SelectedOptionId).ToList();
        var options = await context.ActivityOptions
            .Where(o => optionIds.Contains(o.Id))
            .Include(o => o.ActivityQuestion)
            .ToListAsync(ct);

        // Her cevap için answer ve log oluştur
        foreach (var dto in command.Answers)
        {
            var option = options.FirstOrDefault(o =>
                o.Id == dto.SelectedOptionId &&
                o.ActivityQuestionId == dto.QuestionId);

            if (option is null)
                return Result<SubmitActivityAnswerResponse>.Failure(
                    SystemErrorCodes.InvalidActivityOption, HttpStatusCode.BadRequest);

            var answer = new UserActivityAnswer(
                userId: userId,
                questionId: dto.QuestionId,
                selectedOptionId: dto.SelectedOptionId,
                carbonValue: option.CarbonValue,
                answeredAt: now);
            context.UserActivityAnswers.Add(answer);

            var log = new UserActivityLog(
                userId: userId,
                questionId: dto.QuestionId,
                optionId: dto.SelectedOptionId,
                score: option.CarbonValue,
                selectedOptionId: dto.SelectedOptionId,
                selectedOptionText: option.Text,
                carbonValue: option.CarbonValue);
            context.UserActivityLogs.Add(log);
        }

        await context.SaveChangesAsync(ct);

        // Bugünkü toplam skor
        var totalCarbon = await context.UserActivityLogs
            .Where(x => x.UserId == userId &&
                        x.ActivityDate >= today &&
                        x.ActivityDate < today.AddDays(1))
            .SumAsync(x => x.TotalCarbonScore, ct);

        // Özet listesi
        var answers = options
            .OrderBy(o => o.ActivityQuestion.DisplayOrder)
            .Select(o => new AnswerSummaryDto(
                o.ActivityQuestion.Text,
                o.Text,
                o.CarbonValue))
            .ToList();

        return Result<SubmitActivityAnswerResponse>.Success(
            new SubmitActivityAnswerResponse
            {
                TotalCarbonScore = totalCarbon,
                IsFlowCompleted = true,
                Answers = answers
            });
    }
}