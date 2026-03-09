namespace IzTek.Carbon.Footprint.Application.Features.LogsUser.Queries.GetAuditLogs;

public class GetAuditLogResponse
{
    public string UserName { get; set; }
    public string Operation { get; set; }
    public string TableName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Details { get; set; }
}
