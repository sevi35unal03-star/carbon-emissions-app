namespace IzTek.Carbon.Footprint.Application.Features.Roles.Commands.Create;


public class UpdateRoleCommand 
{
    public Guid Id { get; set; } // Hangi rol güncellenecek?
    public string Name { get; set; } = string.Empty;
    public RoleType Type { get; set; }
}

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