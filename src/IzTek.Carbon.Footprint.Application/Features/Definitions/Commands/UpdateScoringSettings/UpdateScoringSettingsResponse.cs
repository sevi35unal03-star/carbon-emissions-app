namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.UpdateScoringSettings;

public record UpdateScoringSettingsResponse(
    int UpdatedCount,
    DateTime UpdatedAt,
    string Message);