namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create;

// 1. Command (İstek)
public class CreateRoleCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public RoleType Type { get; set; }
}

// 2. Validator (Doğrulayıcı)
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