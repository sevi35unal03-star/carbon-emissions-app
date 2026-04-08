namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            // Product örneğindeki gibi sade kurallar
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid role type.");
        }
    }
}
