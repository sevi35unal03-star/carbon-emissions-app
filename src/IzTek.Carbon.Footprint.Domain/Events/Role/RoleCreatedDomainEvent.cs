namespace IzTek.Carbon.Footprint.Domain.Events.Role;

public class RoleCreatedDomainEvent : BaseEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public RoleType Type { get; set; }

    public RoleCreatedDomainEvent(Role role)
    {
        Id = role.Id;
        Name = role.Name;
        Type = role.Type;
    }
}