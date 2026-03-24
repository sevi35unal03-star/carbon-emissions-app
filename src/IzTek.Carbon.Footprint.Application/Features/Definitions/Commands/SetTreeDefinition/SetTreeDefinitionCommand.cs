namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.SetTreeDefinition;

public record SetTreeDefinitionCommand(
    double PointUnit,
    int TreeCount,
    int GlobalTargetTreeCount);  // ← YENİ