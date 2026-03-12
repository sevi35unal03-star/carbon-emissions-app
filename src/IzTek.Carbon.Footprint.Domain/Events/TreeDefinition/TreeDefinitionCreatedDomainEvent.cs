
namespace IzTek.Carbon.Footprint.Domain.Events.TreeDefinition;

public class TreeDefinitionCreatedDomainEvent(double pointUnit, int treeCount) : BaseEvent 
{
    public double PointUnit { get; } = pointUnit;
    public int TreeCount { get; } = treeCount;
}