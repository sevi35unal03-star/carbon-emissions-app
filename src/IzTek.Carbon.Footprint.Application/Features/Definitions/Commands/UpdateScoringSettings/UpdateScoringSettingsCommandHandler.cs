namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.UpdateScoringSettings;

public static class UpdateScoringSettingsHandler
{
    public static async Task<Result> Handle(
        UpdateScoringSettingsCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var settingIds = command.Settings
            .Select(s => s.Id)
            .ToList();

        var existingSettings = await context.ScoringSettings
            .Where(s => settingIds.Contains(s.Id))
            .ToListAsync(ct);

        if (!existingSettings.Any())
            return Result.Failure(
                SystemErrorCodes.ScoringSettingsNotFound, HttpStatusCode.NotFound);

        var missingIds = settingIds
            .Except(existingSettings.Select(s => s.Id))
            .ToList();

        if (missingIds.Any())
            return Result.Failure(
                SystemErrorCodes.ScoringSettingsPartialNotFound,
                $"Şu ID'ler bulunamadı: {string.Join(", ", missingIds)}",
                HttpStatusCode.NotFound);

        foreach (var setting in existingSettings)
        {
            var newValue = command.Settings
                .First(x => x.Id == setting.Id).Value;

            setting.UpdateValue(newValue);
        }

        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}