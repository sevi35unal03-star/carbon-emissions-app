namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.UpdateScoringSettings;

public record UpdateScoringSettingsCommand(
    List<ScoringSettingUpdateDto> Settings);

public record ScoringSettingUpdateDto(
    Guid Id,
    double Value);

