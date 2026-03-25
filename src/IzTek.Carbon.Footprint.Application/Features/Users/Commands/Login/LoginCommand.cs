namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;

public class LoginCommand
{
    public string EmailorIdentityNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmailorIdentityNumber)
            .NotEmpty().WithMessage("Email or identity number is required.");

        RuleFor(x => x.Password)
           .NotEmpty().WithMessage("Şifreniz boş olamaz.");
    }
}