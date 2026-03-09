namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Password;

public class ResetPasswordCommand
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string ResetCode { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;

    public class Validator : AbstractValidator<ResetPasswordCommand>
    {
        public Validator()
        {
            RuleFor(x => x.PhoneNumber)
            .SetValidator(new PhoneNumberValidator());

            RuleFor(x => x.ResetCode)
                .NotEmpty().WithMessage("Reset code cannot be empty.")
                .Length(5).WithMessage("The reset code must be 5 digits.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[\!\?\*\.]").WithMessage("Password must contain at least one special character (!?*.).");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
        }
    }
}