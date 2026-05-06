namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetPreviousAnswers;

public record GetPreviousAnswersQuery;

public static class GetPreviousAnswersHandler
{
    public static async Task<Result<List<PreviousAnswerGroupDto>>> Handle(
        GetPreviousAnswersQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        CancellationToken ct)
    {
   
        var userId = currentUserService.UserId;

        if (userId is null)
            return Result<List<PreviousAnswerGroupDto>>.Failure(
                SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);

        var logs = await context.UserActivityLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.ActivityDate)
            .Select(x => new PreviousAnswerItemDto(
                x.ActivityQuestion.Text,  
                x.ActivityOption.Text,     
                x.TotalCarbonScore,       
                x.ActivityDate))          
            .ToListAsync(ct);


        if (!logs.Any())
            return Result<List<PreviousAnswerGroupDto>>.Failure(
                SystemErrorCodes.PreviousAnswersNotFound, HttpStatusCode.NotFound);

        var today = DateTime.UtcNow.Date;

        // Logları tarihe göre grupla ve en yeniden eskiye sırala
        // Örnek: { 2025-05-06 → [log1, log2], 2025-05-05 → [log3] }
        var grouped = logs
            .GroupBy(x => x.Date.Date)
            .OrderByDescending(g => g.Key)
            .ToList();

        // Bugün en az bir soru cevaplanmış mı kontrol et
        var hasAnsweredToday = grouped.Any(g => g.Key == today);

        // Bugün cevap verdiyse → bugün + bir önceki günü getir (Take(2))
        // Bugün cevap vermediyse → sadece en son cevaplanan günü getir (Take(1))
        var filtered = hasAnsweredToday
            ? grouped.Take(2)
            : grouped.Take(1);

        // Filtrelenmiş grupları DTO'ya dönüştürz
        var result = filtered
            .Select(g => new PreviousAnswerGroupDto(
                Date: g.Key,           
                Answers: g.ToList()))  
            .ToList();

        // Başarılı sonucu döndür
        return Result<List<PreviousAnswerGroupDto>>.Success(result);
    }
}