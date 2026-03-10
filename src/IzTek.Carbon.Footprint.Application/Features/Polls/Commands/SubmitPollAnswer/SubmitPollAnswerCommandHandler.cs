namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public static class SubmitPollAnswerCommandHandler
{
    public static async Task<Result<SubmitPollAnswerResponse>> HandleAsync(
        SubmitPollAnswerCommand command,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        // 1. Seçilen option Id'lerini al
        var optionIds = command.Answers
            .Select(a => a.OptionId)
            .ToList();

        // 2. Seçilen seçenekleri getir
        var options = await context.PollOptions
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

        // 6. UserPollResult kaydet
        var pollResult = new UserPollResult(
            currentUser.UserId,
            command.PollSetId,
            totalCarbonScore,
            calculatedTrees);

        context.UserPollResults.Add(pollResult);

        // 7. User profilini güncelle
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == currentUser.UserId, ct); 

        if (user is not null)
            user.UpdateMonthlyCarbonResult(totalCarbonScore, calculatedTrees);

        // 8. Kaydet
        await context.SaveChangesAsync(ct);

        return Result<SubmitPollAnswerResponse>.Success(new SubmitPollAnswerResponse( 
            totalCarbonScore,
            calculatedTrees));
    }
}