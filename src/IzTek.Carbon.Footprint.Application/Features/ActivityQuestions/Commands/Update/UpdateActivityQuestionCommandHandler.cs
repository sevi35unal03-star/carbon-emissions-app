using IzTek.Carbon.Footprint.Domain.Common.Exceptions;
using IzTek.Carbon.Footprint.Domain.Events.Activity;

namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Update;

public static class UpdateActivityQuestionCommandHandler
{
    public static async Task<Result> Handle(
        UpdateActivityQuestionCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Soruyu ve Mevcut Seçeneklerini Getir (Tracking Açık)
        var question = await context.ActivityQuestions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (question is null)
            throw new DomainException(SystemErrorCodes.ActivityQuestionNotFound);

        // 2. Ana Alanları Güncelle (Domain Metodu Kullanımı)
        question.UpdateDetails(
            command.Text,
            command.DisplayOrder,
            command.StartDate,
            command.EndDate,
            command.NotificationTime);

        // 3. Seçenekleri Senkronize Et

        // a. Silinenleri Kaldır
        var incomingOptionIds = command.Options
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToList();

        var optionsToRemove = question.Options
            .Where(x => !incomingOptionIds.Contains(x.Id))
            .ToList();

        foreach (var opt in optionsToRemove)
            context.ActivityOptions.Remove(opt);

        // b. Güncelle veya Ekle
        foreach (var optReq in command.Options)
        {
            if (optReq.Id.HasValue)
            {
                var existingOpt = question.Options
                    .FirstOrDefault(x => x.Id == optReq.Id.Value);

                if (existingOpt is null)
                    throw new DomainException(SystemErrorCodes.NotFound);

                existingOpt.UpdateDetails(
                    optReq.Text,
                    optReq.CarbonValue,
                    optReq.NextQuestionId);
            }
            else
            {
                question.AddOption(
                    optReq.Text,
                    optReq.CarbonValue,
                    optReq.NextQuestionId);
            }
        }

        // 4. Domain Event
        question.AddDomainEvent(new ActivityQuestionUpdatedDomainEvent(
            question.Id,
            question.Text,
            question.ScheduledTime));

        // 5. Kaydet
        try
        {
            await context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainException(SystemErrorCodes.Conflict);
        }
    }
}