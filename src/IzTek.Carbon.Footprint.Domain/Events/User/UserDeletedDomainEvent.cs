namespace IzTek.Carbon.Footprint.Domain.Events.User;


public class UserDeletedDomainEvent(Guid userId, DateTime deletedAt) : BaseEvent // ✅ BaseEvent'ten inherit eklendi
{
    public Guid UserId { get; init; } = userId;
    public DateTime DeletedAt { get; init; } = deletedAt;
}