namespace IzTek.Carbon.Footprint.Domain.Common;

public interface IDomainEventContainer
{
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
    void AddDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
}