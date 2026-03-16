namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public static class SubmitPollAnswerCommandHandler
{

    public static async Task<Result<SubmitPollAnswerResponse>> Handle(
    SubmitPollAnswerCommand command,
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    CancellationToken ct)
    {
        // 1. Seçilen option Id'lerini al
        var optionIds = command.Answers
            .Select(a => a.OptionId)
            .ToList();

        // KALDIRILDI — duplicate kontrol validator'da yapılıyor (B-18)

        // 2. Seçilen seçenekleri soru bilgisiyle birlikte getir
        var options = await context.PollOptions
            .Include(o => o.PollQuestion)
            .Where(o => optionIds.Contains(o.Id))
            .ToListAsync(ct);

        if (!options.Any())
            return Result<SubmitPollAnswerResponse>.Failure(
                SystemErrorCodes.InvalidPollAnswers, HttpStatusCode.BadRequest);

        // 3. Aktif TreeDefinition getir
        var treeDef = await context.TreeDefinitions
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        // 4. Toplam karbon skoru hesapla
        double totalCarbonScore = options.Sum(x => x.CarbonValue);

        // 5. Ağaç karşılığını hesapla
        int calculatedTrees = treeDef is not null && treeDef.PointUnit > 0
            ? (int)((totalCarbonScore / treeDef.PointUnit) * treeDef.TreeCount)
            : 0;

        // 6. Kullanıcı bilgilerini getir
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == currentUser.UserId, ct);
        if (user is null)
            return Result<SubmitPollAnswerResponse>.Failure(
                SystemErrorCodes.UserNotFound, HttpStatusCode.NotFound);

        // 7. UserPollResult kaydet
        var pollResult = new UserPollResult(
            name: user.Name,
            surname: user.Surname,
            userId: currentUser.UserId!.Value,
            pollSetId: command.PollSetId,
            totalScore: totalCarbonScore,
            treeCount: calculatedTrees);

        context.UserPollResults.Add(pollResult);

        // 8. Her cevap için UserPollAnswer kaydet
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

        // 9. User profilini güncelle
        user.UpdateMonthlyCarbonResult(totalCarbonScore);

        // 10. Kaydet
        await context.SaveChangesAsync(ct);

        return Result<SubmitPollAnswerResponse>.Success(new SubmitPollAnswerResponse(
            totalCarbonScore,
            calculatedTrees));
    }
}