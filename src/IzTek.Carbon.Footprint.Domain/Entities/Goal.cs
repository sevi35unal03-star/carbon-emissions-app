namespace IzTek.Carbon.Footprint.Domain.Entities;

public class Goal : BaseAuditableEntity
{
    public int Month { get; private set; }
    public int Year { get; private set; }
    public int TargetTreeCount { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedDate { get; private set; }

    private Goal() { }

    public Goal(int month, int year, int targetTreeCount)
    {
        Month = month;
        Year = year;
        TargetTreeCount = targetTreeCount;
        IsCompleted = false;
    }

    public void Complete()
    {
        IsCompleted = true;
        CompletedDate = DateTime.UtcNow;
    }

    public void UpdateTarget(int targetTreeCount)
    {
        TargetTreeCount = targetTreeCount;
    }
}