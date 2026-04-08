using IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create;

namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Update
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Role ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name cannot be empty.")
                .MaximumLength(128).WithMessage("Role name is too long.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Please provide a valid role type.");
        }
    }
}
