namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.SetTreeDefinition;

public record SetTreeDefinitionResponse(
    double PointUnit,
    int TreeCount,
    int GlobalTargetTreeCount);  // ← YENİ