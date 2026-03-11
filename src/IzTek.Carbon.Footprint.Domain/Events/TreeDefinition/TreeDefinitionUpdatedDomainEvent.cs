

namespace IzTek.Carbon.Footprint.Domain.Events.TreeDefinition;

public class TreeDefinitionUpdatedDomainEvent : BaseEvent
{
    public TreeDefinitionUpdatedDomainEvent(double pointUnit, int treeCount)
    {
        PointUnit = pointUnit;
        TreeCount = treeCount;
    }

    public double PointUnit { get; }
    public int TreeCount { get; }
}