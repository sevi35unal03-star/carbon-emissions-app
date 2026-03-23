

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string UserId { get; set; } = default!;

    public string UserName { get; set; } = default!;
    public string Operation { get; set; } = default!;
    public string TableName { get; set; } = default!;
    public string? OldValues { get; set; } = default!;
    public string? NewValues { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    private AuditLog() { }

    public AuditLog(string userId, string userName, string operation, string tableName, string? oldValues, string? newValues, DateTime createdAt)
    {
        UserId = userId;
        UserName = userName;
        Operation = operation;
        TableName = tableName;
        OldValues = oldValues;
        NewValues = newValues;
        CreatedAt = createdAt;
    }
}