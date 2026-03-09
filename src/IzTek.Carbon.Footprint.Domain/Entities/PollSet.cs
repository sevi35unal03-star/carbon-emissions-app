using IzTek.Carbon.Footprint.Application.Features.Polls.Events;
using IzTek.Carbon.Footprint.Domain.Events.Poll.Create;

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class PollSet : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    private readonly List<PollQuestion> _questions = new();
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
        //AddDomainEvent(new PollSetCreatedDomainEvent(Id, name)); // ✅ Aktif edildi
    }

    public void UpdateDetails(string name, string description, int displayOrder)
    {
        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
        //AddDomainEvent(new PollSetUpdatedDomainEvent(Id, name)); // ✅ Aktif edildi
    }

    public void Activate()
    {
        IsActive = true;
        //AddDomainEvent(new PollSetActivatedDomainEvent(Id)); // ✅ Aktif edildi
    }

    public void Deactivate()
    {
        IsActive = false;
        //AddDomainEvent(new PollSetDeactivatedDomainEvent(Id)); // ✅ Aktif edildi
    }

    public void AddQuestion(string text, int displayOrder)
    {
        _questions.Add(new PollQuestion(Id, text, displayOrder));
    }

    public void UpdateDetails(string name, object description, object displayOrder)
    {
        throw new NotImplementedException();
    }
}