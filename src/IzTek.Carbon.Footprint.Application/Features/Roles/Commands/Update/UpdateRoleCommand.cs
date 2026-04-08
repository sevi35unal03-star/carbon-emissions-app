namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create;


public class UpdateRoleCommand 
{
    public Guid Id { get; set; } // Hangi rol güncellenecek?
    public string Name { get; set; } = string.Empty;
    public RoleType Type { get; set; }
}

