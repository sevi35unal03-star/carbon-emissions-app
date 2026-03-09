namespace IzTek.Carbon.Footprint.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string UserId { get; set; }        
    public string Operation { get; set; }    
    public string TableName { get; set; }     
    public string OldValues { get; set; }     
    public string NewValues { get; set; }      
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    private AuditLog() { }

    public AuditLog(string userId, string operation, string tableName, string oldValues, string newValues, DateTime createdAt)
    {
        UserId = userId;
        Operation = operation;
        TableName = tableName;
        OldValues = oldValues;
        NewValues = newValues;
        CreatedAt = createdAt;
    }
}

