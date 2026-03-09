using IzTek.Carbon.Footprint.Domain.Common;

namespace IzTek.Carbon.Footprint.Domain.Events.UsefulInformation;

public record UsefulInformationUpdatedDomainEvent : BaseEvent
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string Content { get; init; } = default!;
    public int DisplayOrder { get; init; }

    public UsefulInformationUpdatedDomainEvent(Guid id, string title, string content, int displayOrder)
    {
        Id = id;
        Title = title;
        Content = content;
        DisplayOrder = displayOrder;
    }
}