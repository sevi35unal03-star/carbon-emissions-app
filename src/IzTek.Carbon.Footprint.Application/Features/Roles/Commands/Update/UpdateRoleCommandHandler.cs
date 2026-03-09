namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create;

public class UpdateRoleCommandHandler(RoleManager<Role> roleManager)
    : IRequestHandler<UpdateRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        // 1. Rolü veritabanından bul
        var role = await roleManager.FindByIdAsync(request.Id.ToString());

        if (role == null)
            return Result.Failure("Role not found.");

        // 2. Değerleri güncelle
        role.Name = request.Name;
        role.Type = request.Type;

        // 3. Identity üzerinden kaydet
        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            var firstError = result.Errors.First().Description;
            return Result.Failure(firstError);
        }

        return Result.Success();
    }
}