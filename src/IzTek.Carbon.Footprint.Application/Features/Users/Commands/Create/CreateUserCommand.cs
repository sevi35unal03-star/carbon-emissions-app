using IzTek.Carbon.Footprint.Application.Common.Validators;
using IzTek.Carbon.Footprint.Domain.Common;


namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Create;

public class CreateUserCommand : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdentityNumber { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    public bool IsKvkkApproved { get; set; }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.IdentityNumber)
            .Must(IdentityValidator.IsValidTurkishIdentityNumber).WithMessage("Identity number must be valid.");

        RuleFor(x => x.Email)
            .Must(EmailValidator.IsValid).WithMessage("Email must be valid.");

        RuleFor(x => x.PhoneNumber)
            .Must(PhoneNumberValidator.IsValidTurkishMobile)
                .WithMessage("Please enter a valid Turkish phone number. (Örn: +905551234567)")
            .Must(PhoneNumberValidator.IsAllowedRegion)
                .WithMessage("Phone number from this region is not allowed.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birth date is required.")
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.ConfirmPassword)
           .NotEmpty().WithMessage("Confirming password is required.")
           .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.IsKvkkApproved)
            .Equal(true).WithMessage("You must accept the KVKK agreement to register.");
    }
}