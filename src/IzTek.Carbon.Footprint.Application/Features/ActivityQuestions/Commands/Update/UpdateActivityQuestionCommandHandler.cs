namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Update;

public static class UpdateActivityQuestionCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateActivityQuestionCommand command,
        IApplicationDbContext context,
        ICacheService cacheService,
        CancellationToken ct)
    {
        // 1. Soruyu ve Mevcut Seçeneklerini Getir (Tracking Açık)
        var question = await context.ActivityQuestions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.PollQuestionId == command.Id, ct);

        if (question == null)
            return Result.Failure(SystemErrorCodes.ActivityQuestionNotFound, HttpStatusCode.NotFound);

        // 2. Ana Alanları Güncelle (Domain Metodu Kullanımı)
        // Private setter'ları aşmak için reflection yerine bu metodu kullanmalısın
        question.UpdateDetails(
            command.Text,
            command.DisplayOrder,
            command.StartDate,
            command.EndDate,
            command.NotificationTime);

        // 3. Seçenekleri Senkronize Et (Complex Sync Logic)

        // a. Silinenleri Kaldır: Gelen listede olmayan ID'leri tespit et
        var incomingOptionIds = command.Options
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToList();

        var optionsToRemove = question.Options
            .Where(x => !incomingOptionIds.Contains(x.PollQuestionId))
            .ToList();

        foreach (var opt in optionsToRemove)
        {
            // Bu seçeneklere verilmiş cevap (SubmitAnswer) varsa silmek hata verebilir, 
            // ama admin panelinde "sil" dendiyse context üzerinden kaldırıyoruz.
            context.ActivityOptions.Remove(opt);
        }

        // b. Güncelle veya Ekle
        foreach (var optReq in command.Options)
        {
            if (optReq.Id.HasValue)
            {
                // Mevcut olanı bul ve güncelle (Kırılım/NextQuestionId dahil)
                var existingOpt = question.Options.FirstOrDefault(x => x.PollQuestionId == optReq.Id.Value);
                existingOpt?.UpdateDetails(optReq.Text, optReq.CarbonValue, optReq.NextQuestionId);
            }
            else
            {
                // Yeni olanı domain metodu ile listeye ekle
                question.AddOption(optReq.Text, optReq.CarbonValue, optReq.NextQuestionId);
            }
        }

        // 4. Domain Event: Zamanlayıcı (Push Notification) güncellenmeli
        // NotificationTime veya Text değiştiyse yeni bir push planlanması tetiklenir
        question.AddDomainEvent(new ActivityQuestionUpdatedDomainEvent(question.PollQuestionId));

        var result = await context.SaveChangesAsync(ct);

        if (result > 0)
        {
            await cacheService.RemoveAsync("all_activity_questions");
            return Result.Success();
        }

        return Result.Failure(SystemErrorCodes.UpdateFailed, HttpStatusCode.InternalServerError);
    }
}