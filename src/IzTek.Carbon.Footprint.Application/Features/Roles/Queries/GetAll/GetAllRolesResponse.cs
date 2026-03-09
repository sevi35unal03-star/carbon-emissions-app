namespace IzTek.Carbon.Footprint.Application.Features.Roles.Queries.GetAll;

public record GetAllRolesResponse
{
    public Guid Id { get; init; }
    // default başlangıçta bu değeri null yap demektir.
    // = default! kullanmak bellekte gereksiz yer kaplamaz.
    public string Name { get; init; } = default!;
    public RoleType Type { get; init; }
}