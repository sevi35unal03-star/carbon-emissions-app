using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace IzTek.Carbon.Footprint.Persistence.Interceptors;

public class AuditEntry(EntityEntry entry)
{
    public EntityEntry Entry { get; } = entry;
    public string UserId { get; set; } = null!; 
    public string UserName { get; set; } = null!;   
    public string TableName { get; set; } = null!;
    public string Operation { get; set; } = null!; // "Create", "Update", "Delete"
    public Dictionary<string, object> OldValues { get; } = [];
    public Dictionary<string, object> NewValues { get; } = [];

    public AuditLog ToAuditLog() => new(
    userId: UserId,
    userName: UserName,
    operation: Operation,
    tableName: TableName,
    oldValues: OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
    newValues: NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues),
    createdAt: DateTime.UtcNow
);
}