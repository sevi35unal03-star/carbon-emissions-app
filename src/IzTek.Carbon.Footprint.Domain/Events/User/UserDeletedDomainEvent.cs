namespace IzTek.Carbon.Footprint.Domain.Events.User;

public record UserDeletedDomainEvent(Guid UserId, DateTime DeletedAt);