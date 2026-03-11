namespace IzTek.Carbon.Footprint.Application.Features.LogsUser.Queries;

public class GetAuditLogsQueryHandler
{
    public async Task<PagedResult<List<GetAuditLogResponse>>> HandleAsync(
        GetAuditLogsQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Temel sorguyu oluştur
        var baseQuery = context.AuditLogs.AsNoTracking();

        // 2. Toplam kayıt sayısını al
        var totalCount = await baseQuery.CountAsync(ct);

        // 3. Sayfala ve map et
        var data = await baseQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new GetAuditLogResponse
            {
                UserName = x.UserName,
                Operation = x.Operation,
                TableName = x.TableName,
                CreatedAt = x.CreatedAt,
                Details = $"Eski: {x.OldValues} -> Yeni: {x.NewValues}"
            })
            .ToListAsync(ct);

        // 4. PagedResult döndür
        return PagedResult<List<GetAuditLogResponse>>.Success(
            data: data,
            totalCount: totalCount,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize
        );
    }
}