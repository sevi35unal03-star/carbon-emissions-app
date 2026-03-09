namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create;

public static class CreateRoleCommandHandler
{
    public static async Task<Result> HandleAsync(
        CreateRoleCommand command,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        // 1. Aynı isimde bir rol var mı kontrolü (Product örneğindeki gibi)
        var isExists = await context.Roles
            .AnyAsync(x => x.Name == command.Name, cancellationToken);

        if (isExists)
        {
            // Not: SystemErrorCodes içine 'RoleAlreadyExists' eklemen gerekebilir.
            return Result.Failure(SystemErrorCodes.RoleAlreadyExists, System.Net.HttpStatusCode.BadRequest);
        }

        // 2. Yeni Role nesnesini Domain kurallarına göre oluştur
        var role = new Role(command.Name, command.Type);

        // 3. Veritabanı setine ekle
        await context.Roles.AddAsync(role, cancellationToken);

        // 4. Domain Event fırlat (Role.cs içinde constructor'a eklemediysek buradan da eklenebilir)
        role.AddDomainEvent(new RoleCreatedDomainEvent()
        {
            Id = role.Id,
            Name = role.Name,
            Type = role.Type
        });

        // 5. Değişiklikleri kaydet ve sonucu dön
        return await context.SaveChangesAsync(cancellationToken) > 0
            ? Result.Created()
            : Result.SystemException();
    }
}