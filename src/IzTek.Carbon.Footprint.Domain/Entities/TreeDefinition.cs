using IzTek.Carbon.Footprint.Domain.Events.TreeDefinition;

public class TreeDefinition : BaseAuditableEntity
{
    public readonly object LastModifiedAt;
    public readonly object LastModifiedBy;

    public double PointUnit { get; private set; }   // Örn: 10
    public int TreeCount { get; private set; }       // Örn: 2
    public bool IsActive { get; private set; }

    // ✅ Eklenmesi gereken: X puan = kaç ağaç?
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