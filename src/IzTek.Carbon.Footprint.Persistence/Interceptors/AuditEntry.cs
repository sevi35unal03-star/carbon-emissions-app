using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace IzTek.Carbon.Footprint.Persistence.Interceptors;

public class AuditEntry
{
    public AuditEntry(EntityEntry entry) => Entry = entry;
    public EntityEntry Entry { get; }
    public string UserId { get; set; }
    public string TableName { get; set; }
    public string Operation { get; set; }
    public Dictionary<string, object> OldValues { get; } = new();
    public Dictionary<string, object> NewValues { get; } = new();

    public AuditLog ToAuditLog() => new AuditLog(
    userId: UserId,
    operation: Operation,
    tableName: TableName,
    oldValues: OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
    newValues: NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues),
    createdAt: DateTime.UtcNow
);
}