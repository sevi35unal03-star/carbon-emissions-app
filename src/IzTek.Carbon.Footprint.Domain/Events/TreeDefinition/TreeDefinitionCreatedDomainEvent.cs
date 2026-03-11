
namespace IzTek.Carbon.Footprint.Domain.Events.TreeDefinition;

public class TreeDefinitionCreatedDomainEvent : BaseEvent 
{
    public TreeDefinitionCreatedDomainEvent(double pointUnit, int treeCount)
    {
        PointUnit = pointUnit;
        TreeCount = treeCount;
    }

    public double PointUnit { get; }
    public int TreeCount { get; }
}