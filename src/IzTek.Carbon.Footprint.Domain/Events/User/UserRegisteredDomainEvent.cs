namespace IzTek.Carbon.Footprint.Domain.Events.User;

public class UserRegisteredDomainEvent(Guid userId, string email, string fullName) : BaseEvent
{
    public Guid UserId { get; init; } = userId;
    public string Email { get; init; } = email;
    public string FullName { get; init; } = fullName;
}