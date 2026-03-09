namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create;

public class DeleteRoleCommandHandler(
    RoleManager<Role> roleManager,
    ICurrentUserService currentUserService)
    : IRequestHandler<DeleteRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        // 1. Rolü bul (Zaten silinmemiş olanları getir)
        var role = await roleManager.FindByIdAsync(request.Id.ToString());

        if (role == null || role.IsDeleted)
            return Result.Failure("Role not found or already deleted.");

        // 2. Kritik rol kontrolü
        if (role.Name == "Admin")
            return Result.Failure("System protected roles cannot be deleted.");

        // 3. Soft Delete işaretlemesi
        role.IsDeleted = true;
        role.DeletedDate = DateTime.UtcNow;
        role.DeletedBy = currentUserService.UserId.ToString(); // Kimin sildiği bilgisi

        // 4. Domain Event ekle
        role.AddDomainEvent(new RoleDeletedDomainEvent(role.Id, role.Name!));

        // 5. Güncelle (Fiziksel silme yok, sadece update)
        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
            return Result.Failure(result.Errors.First().Description);

        return Result.Success();
    }
}