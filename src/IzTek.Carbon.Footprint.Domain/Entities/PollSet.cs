namespace IzTek.Carbon.Footprint.Domain.Entities;

public class PollSet : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    private readonly List<PollQuestion> _questions = [];
    public IReadOnlyCollection<PollQuestion> Questions => _questions.AsReadOnly();

    private PollSet() { }

    public PollSet(string name, string description, int displayOrder, int month, int year)
    {
        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
        Month = month;
        Year = year;
        IsActive = true;
    }

    public void UpdateDetails(string name, string description, int displayOrder)
    {
        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void AddQuestion(string text, int displayOrder)
    {
        _questions.Add(new PollQuestion(Id, text, displayOrder));
    }
}