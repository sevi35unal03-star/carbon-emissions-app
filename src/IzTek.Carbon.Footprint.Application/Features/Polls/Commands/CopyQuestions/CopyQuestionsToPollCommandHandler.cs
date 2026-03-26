namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.CopyQuestions;

public static class CopyQuestionsToPollCommandHandler
{
    public static async Task<Result> Handle(
        CopyQuestionsToPollCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Boş liste kontrolü
        if (command.SourceQuestionIds is null || !command.SourceQuestionIds.Any())
            return Result.Failure(SystemErrorCodes.SourceQuestionIdsEmpty, HttpStatusCode.BadRequest);

        // 2. Kaynak soruları ve seçeneklerini getir
        var sourceQuestions = await context.ActivityQuestions
            .AsNoTracking()
            .Include(x => x.Options)
            .Where(x => command.SourceQuestionIds.Contains(x.Id))
            .ToListAsync(ct);

        // 3. Kısmi eşleşme kontrolü
        var missingIds = command.SourceQuestionIds
            .Except(sourceQuestions.Select(x => x.Id))
            .ToList();

        if (missingIds.Count != 0)
            return Result.Failure(
                SystemErrorCodes.SourceQuestionsNotFound,
                $"Şu ID'lere ait sorular bulunamadı: {string.Join(", ", missingIds)}",
                HttpStatusCode.NotFound);

        // 4. Her soruyu domain factory metodu ile klonla
        var pollQuestions = sourceQuestions
            .Select(sourceQ => PollQuestion.CloneFrom(sourceQ, command.PollSetId))
            .ToList();

        await context.PollQuestions.AddRangeAsync(pollQuestions, ct);
        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}