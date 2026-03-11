namespace IzTek.Carbon.Footprint.Application.Features.LogsUser.Queries;

public record GetAuditLogsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    bool? ShowDeleted = null);