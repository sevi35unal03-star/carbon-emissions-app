using IzTek.Carbon.Footprint.Domain.Events.TreeDefinition;

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class TreeDefinition : BaseAuditableEntity
{
    public double PointUnit { get; private set; }
    public int TreeCount { get; private set; }
    public int GlobalTargetTreeCount { get; private set; }  // ← YENİ: Admin belirler

    public double CalculateTreeCount(double totalPoints)
    {
        if (PointUnit <= 0) return 0;
        return (totalPoints / PointUnit) * TreeCount;
    }

    private TreeDefinition() { }

    public TreeDefinition(double pointUnit, int treeCount, int globalTargetTreeCount)
    {
        PointUnit = pointUnit;
        TreeCount = treeCount;
        GlobalTargetTreeCount = globalTargetTreeCount;
        IsActive = true;
        AddDomainEvent(new TreeDefinitionCreatedDomainEvent(pointUnit, treeCount));
    }

    public void Update(double pointUnit, int treeCount, int globalTargetTreeCount)
    {
        PointUnit = pointUnit;
        TreeCount = treeCount;
        GlobalTargetTreeCount = globalTargetTreeCount;
        AddDomainEvent(new TreeDefinitionUpdatedDomainEvent(pointUnit, treeCount));
    }
}
