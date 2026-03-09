namespace IzTek.Carbon.Footprint.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }
    public RoleType Type { get; set; }
