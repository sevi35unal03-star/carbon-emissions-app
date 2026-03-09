namespace IzTek.Carbon.Footprint.Domain.Events.Role;

public record RoleUpdatedDomainEvent(Guid Id, string Name, RoleType Type) : BaseEvent;