namespace IzTek.Carbon.Footprint.Domain.Events;

public record UsefulInformationCreatedDomainEvent : BaseEvent
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
}
