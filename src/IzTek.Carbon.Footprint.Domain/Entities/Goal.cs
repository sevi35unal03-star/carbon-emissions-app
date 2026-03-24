namespace IzTek.Carbon.Footprint.Domain.Entities;

public class Goal : BaseAuditableEntity
{
    public Guid? UserId { get; private set; }  // null = global hedef
    public int Month { get; private set; }
    public int Year { get; private set; }
    public int TargetTreeCount { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedDate { get; private set; }

    private Goal() { }

    // Kişisel hedef — kullanıcı oluşturur
    public Goal(Guid userId, int month, int year, int targetTreeCount)
    {
        UserId = userId;
        Month = month;
        Year = year;
        TargetTreeCount = targetTreeCount;
        IsCompleted = false;
    }

    // Global hedef — admin oluşturur, UserId = null
    public Goal(int month, int year, int targetTreeCount)
    {
        UserId = null;
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