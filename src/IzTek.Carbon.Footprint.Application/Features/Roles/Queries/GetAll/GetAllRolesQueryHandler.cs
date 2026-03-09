namespace IzTek.Carbon.Footprint.Application.Features.Roles.Queries.GetAll;

public static class GetAllRolesQueryHandler
{
    public static async Task<Result<List<GetAllRolesResponse>>> HandleAsync(
        GetAllRolesQuery query,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        // 1. Veritabanından rolleri seçip Response formatına dönüştürüyoruz (Projection)
        var roles = await context.Roles
            .Select(x => new GetAllRolesResponse()
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type // Enum olarak gelecek, Response içinde string ise .ToString() eklenebilir
            })
            .ToListAsync(cancellationToken);

        // 2. Sonucu Result deseniyle paketleyip dönüyoruz
        return Result<List<GetAllRolesResponse>>.Success(roles);
    }
}