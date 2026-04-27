namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create;

public static class CreateRoleCommandHandler
{
    public static async Task<Result> Handle(
        CreateRoleCommand command,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        // 1. Aynı isimde bir rol var mı kontrolü 
        var isExists = await context.Roles
            .AnyAsync(x => x.Name == command.Name, cancellationToken);

        if (isExists)
        {
            return Result.Failure(SystemErrorCodes.RoleAlreadyExists, System.Net.HttpStatusCode.BadRequest);
        }

        // 2. Yeni Role nesnesini Domain kurallarına göre oluştur
        var role = new Role(command.Name, command.Type);

        // 3. Veritabanı setine ekle
        await context.Roles.AddAsync(role, cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? Result.Created()
            : Result.SystemException();
    }
}