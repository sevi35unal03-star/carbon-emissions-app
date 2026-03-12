namespace IzTek.Carbon.Footprint.Application.Features.LogsUser.Queries;

public class GetAuditLogResponse
{
    public string UserName { get; set; } = null!;
    public string Operation { get; set; } = null!;
    public string TableName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Details { get; set; } = null!;
}
