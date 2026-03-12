

namespace IzTek.Carbon.Footprint.Domain.Events.TreeDefinition;

public class TreeDefinitionUpdatedDomainEvent(double pointUnit, int treeCount) : BaseEvent
{
    public double PointUnit { get; } = pointUnit;
    public int TreeCount { get; } = treeCount;
}