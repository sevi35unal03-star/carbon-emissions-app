using Microsoft.AspNetCore.Identity;

namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create;

public class UpdateRoleCommandHandler(RoleManager<Role> roleManager)
   
{
    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        // 1. Rolü veritabanından bul
        var role = await roleManager.FindByIdAsync(request.Id.ToString());

        if (role == null)
            return Result.Failure(SystemErrorCodes.RoleNotFound, HttpStatusCode.NotFound);

        // 2. Değerleri güncelle
        role.Name = request.Name;
        role.Type = request.Type;

        // 3. Identity üzerinden kaydet
        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            
            return Result.Failure(SystemErrorCodes.BadRequest, HttpStatusCode.BadRequest);
        }

        return Result.Success();
    }
}