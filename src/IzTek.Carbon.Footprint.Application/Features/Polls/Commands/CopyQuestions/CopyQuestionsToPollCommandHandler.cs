
namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.CopyQuestions;

public class CopyQuestionsToPollCommandHandler(IApplicationDbContext context)
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> Handle(CopyQuestionsToPollCommand command, CancellationToken ct)
    {
        // 1. Boş liste kontrolü
        if (command.SourceQuestionIds is null || !command.SourceQuestionIds.Any())
            return Result.Failure(SystemErrorCodes.SourceQuestionIdsEmpty, HttpStatusCode.BadRequest);

        // 2. Kaynak soruları ve seçeneklerini getir
        var sourceQuestions = await _context.ActivityQuestions
            .AsNoTracking()
            .Include(x => x.Options)
            .Where(x => command.SourceQuestionIds.Contains(x.Id)) // ✅ PollQuestionId → Id
            .ToListAsync(ct);

        // 3. Kısmi eşleşme kontrolü
        var missingIds = command.SourceQuestionIds
            .Except(sourceQuestions.Select(x => x.Id)) // ✅ PollQuestionId → Id
            .ToList();

        if (missingIds.Any())
            return Result.Failure(
                SystemErrorCodes.SourceQuestionsNotFound,
                $"Şu ID'lere ait sorular bulunamadı: {string.Join(", ", missingIds)}",
                HttpStatusCode.NotFound);

        // 4. Her soruyu domain factory metodu ile klonla
        var pollQuestions = sourceQuestions
            .Select(sourceQ => PollQuestion.CloneFrom(sourceQ, command.PollSetId))
            .ToList();

        await _context.PollQuestions.AddRangeAsync(pollQuestions, ct);
        await _context.SaveChangesAsync(ct);

        return Result.Success();
    }
}