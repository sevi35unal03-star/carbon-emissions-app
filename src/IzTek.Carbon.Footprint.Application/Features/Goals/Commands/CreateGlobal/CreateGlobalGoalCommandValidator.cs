// CreateGlobalGoalCommandValidator.cs
using FluentValidation;

namespace IzTek.Carbon.Footprint.Application.Features.Goals.Commands.CreateGlobal;

public class CreateGlobalGoalCommandValidator : AbstractValidator<CreateGlobalGoalCommand>
{
    public CreateGlobalGoalCommandValidator()
    {
        RuleFor(x => x.Month)
            .InclusiveBetween(0, 12)
            .WithMessage("Ay 0 (yıllık hedef) ile 12 arasında olmalıdır.");

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(2000)
            .LessThanOrEqualTo(2100)
            .WithMessage("Geçerli bir yıl giriniz.");

        RuleFor(x => x.TargetTreeCount)
            .GreaterThan(0)
            .WithMessage("Hedef ağaç sayısı 0'dan büyük olmalıdır.");
    }
}