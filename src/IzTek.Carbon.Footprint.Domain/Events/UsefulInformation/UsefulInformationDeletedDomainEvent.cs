using IzTek.Carbon.Footprint.Domain.Common;

namespace IzTek.Carbon.Footprint.Domain.Events.UsefulInformation;

public record UsefulInformationDeletedDomainEvent : BaseEvent
{
    public Guid Id { get; init; }

    public UsefulInformationDeletedDomainEvent(Guid id)
    {
        Id = id;
    }
}