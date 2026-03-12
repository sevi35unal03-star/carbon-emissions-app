using IzTek.Carbon.Footprint.Application.Common.Constants;

namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.UpdateScoringSettings;

public class UpdateScoringSettingsHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public UpdateScoringSettingsHandler(
        IApplicationDbContext context,
        ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result> HandleAsync(
        UpdateScoringSettingsCommand command,
        CancellationToken ct)
    {
        // 1. Gelen ID'leri al
        var settingIds = command.Settings
            .Select(s => s.Id)
            .ToList();

        // 2. DB'den mevcut kayıtları getir
        var existingSettings = await _context.ScoringSettings
            .Where(s => settingIds.Contains(s.Id))
            .ToListAsync(ct);

        if (!existingSettings.Any())
            return Result.Failure(
                SystemErrorCodes.ScoringSettingsNotFound, HttpStatusCode.NotFound);

        // 3. Kısmi güncelleme kontrolü
        var missingIds = settingIds
            .Except(existingSettings.Select(s => s.Id))
            .ToList();

        if (missingIds.Any())
            return Result.Failure(
                SystemErrorCodes.ScoringSettingsPartialNotFound,
                $"Şu ID'ler bulunamadı: {string.Join(", ", missingIds)}",
                HttpStatusCode.NotFound);

        // 4. Domain metodu ile güncelle (Encapsulation)
        foreach (var setting in existingSettings)
        {
            var newValue = command.Settings
                .First(x => x.Id == setting.Id).Value;

            setting.UpdateValue(newValue);
        }

        // 5. Kaydet
        await _context.SaveChangesAsync(ct);

        // 6. Cache temizle
        await _cacheService.RemoveAsync(CacheKeys.GlobalScoringParams);

        return Result.Success();
    }
}