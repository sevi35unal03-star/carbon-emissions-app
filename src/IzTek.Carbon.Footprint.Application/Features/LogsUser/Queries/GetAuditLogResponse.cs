namespace IzTek.Carbon.Footprint.Application.Features.LogsUser.Queries;

public class GetAuditLogResponse
{
    public string UserName { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Details { get; set; } = string.Empty;
}
