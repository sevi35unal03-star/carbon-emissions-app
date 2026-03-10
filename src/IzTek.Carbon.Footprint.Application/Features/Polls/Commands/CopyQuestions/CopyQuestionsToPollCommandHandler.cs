namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.CopyQuestions;

public class CopyQuestionsToPollCommandHandler(IApplicationDbContext context)
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(
        CopyQuestionsToPollRequest request,
        CancellationToken ct)
    {
        // 1. Boş liste kontrolü
        if (request.SourceQuestionIds is null || !request.SourceQuestionIds.Any())
            return Result.Failure(
                SystemErrorCodes.SourceQuestionIdsEmpty, HttpStatusCode.BadRequest);

        // 2. Kaynak soruları ve seçeneklerini getir
        var sourceQuestions = await _context.ActivityQuestions
            .AsNoTracking()
            .Include(x => x.Options)
            .Where(x => request.SourceQuestionIds.Contains(x.PollQuestionId))
            .ToListAsync(ct);

        // 3. Kısmi eşleşme kontrolü
        var missingIds = request.SourceQuestionIds
            .Except(sourceQuestions.Select(x => x.PollQuestionId))
            .ToList();

        if (missingIds.Any())
            return Result.Failure(
                SystemErrorCodes.SourceQuestionsNotFound,
                $"Şu ID'lere ait sorular bulunamadı: {string.Join(", ", missingIds)}",
                HttpStatusCode.NotFound);

        // 4. Her soruyu domain factory metodu ile klonla
        var pollQuestions = sourceQuestions
            .Select(sourceQ => PollQuestion.CloneFrom(sourceQ, request.PollSetId))
            .ToList();

        await _context.PollQuestions.AddRangeAsync(pollQuestions, ct);
        await _context.SaveChangesAsync(ct);

        return Result.Success();
    }
}