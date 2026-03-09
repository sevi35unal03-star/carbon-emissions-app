namespace IzTek.Carbon.Footprint.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
}