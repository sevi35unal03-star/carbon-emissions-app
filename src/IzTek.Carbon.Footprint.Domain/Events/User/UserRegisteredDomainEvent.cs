namespace IzTek.Carbon.Footprint.Domain.Events.User;

public class UserRegisteredDomainEvent : BaseEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FullName { get; }

    public UserRegisteredDomainEvent(Guid userId, string email, string fullName)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
    }
}