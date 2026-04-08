using IzTek.Carbon.Footprint.Application.Common.Validators;
namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .Must(PhoneNumberValidator.IsValidTurkishMobile)
                .WithMessage("Geçerli bir telefon numarası giriniz. (Örn: +905551234567)");
        }
    }
}
