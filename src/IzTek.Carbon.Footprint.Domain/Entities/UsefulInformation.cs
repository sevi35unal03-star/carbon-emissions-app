namespace IzTek.Carbon.Footprint.Domain.Entities;

public class UsefulInformation : BaseAuditableEntity
{
    public string Title { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public int DisplayOrder { get; private set; }
    public new bool IsActive { get; private set; } // ✅ new keyword eklendi

    private UsefulInformation() { }

    public UsefulInformation(string title, string content, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty.");
        Title = title;
        Content = content;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public void Update(string title, string content, int displayOrder)
    {
        Title = title;
        Content = content;
        DisplayOrder = displayOrder;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}