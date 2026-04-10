using Microsoft.EntityFrameworkCore;

namespace IzTek.Carbon.Footprint.Application.Features.LogsUser.Queries;

public static class GetAuditLogsQueryHandler
{
    public static async Task<PagedResult<List<GetAuditLogResponse>>> Handle(
        GetAuditLogsQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var baseQuery = context.AuditLogs.AsNoTracking();

        // 🔍 Search filtresi (minimal ama etkili)
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var search = query.SearchTerm.ToLower();

            baseQuery = baseQuery.Where(x =>
                x.UserName.ToLower().Contains(search) ||
                x.Operation.ToLower().Contains(search) ||
                x.TableName.ToLower().Contains(search));
        }

        // 📊 Filtre sonrası toplam kayıt
        var totalCount = await baseQuery.CountAsync(ct);

        // 📥 Verileri çek
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

        return PagedResult<List<GetAuditLogResponse>>.Success(
            data: data,
            totalCount: totalCount,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize);
    }
}