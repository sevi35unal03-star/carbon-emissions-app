using IzTek.Carbon.Footprint.Domain.Events.TreeDefinition;

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class TreeDefinition : BaseAuditableEntity
{
    // LastModifiedAt/By kaldırıldı — BaseAuditableEntity'de UpdatedAt/UpdatedBy olarak zaten var
    public double PointUnit { get; private set; }
    public int TreeCount { get; private set; }

    public double CalculateTreeCount(double totalPoints)
    {
        if (PointUnit <= 0) return 0;
        return (totalPoints / PointUnit) * TreeCount;
    }

    private TreeDefinition() { }

    public TreeDefinition(double pointUnit, int treeCount)
    {
        PointUnit = pointUnit;
        TreeCount = treeCount;
        IsActive = true;
        AddDomainEvent(new TreeDefinitionCreatedDomainEvent(pointUnit, treeCount));
    }

    public void Update(double pointUnit, int treeCount)
    {
        PointUnit = pointUnit;
        TreeCount = treeCount;
        AddDomainEvent(new TreeDefinitionUpdatedDomainEvent(pointUnit, treeCount));
    }
}