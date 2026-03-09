namespace IzTek.Carbon.Footprint.Domain.Entities;

public class ScoringSetting : BaseAuditableEntity
{
    public string Key { get; private set; }
    public double Value { get; private set; }
    public ScoringCategory Category { get; private set; } // Örn: Transport, Energy...

    private ScoringSetting() { } // EF Core için

    public ScoringSetting(string key, double value, ScoringCategory category)
    {
        Key = key;
        Value = value;
        Category = category;
    }

    public void UpdateValue(double newValue)
    {
        Value = newValue;
    }
}