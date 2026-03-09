namespace IzTek.Carbon.Footprint.Domain.Events.Role;

public record RoleDeletedDomainEvent(Guid Id, string Name) : BaseEvent;