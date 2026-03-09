namespace IzTek.Carbon.Footprint.Application.Features.Definitions.Commands.UpdateScoringSettings;

public record UpdateScoringSettingsCommand(
    List<ScoringSettingUpdateDto> Settings);

public record ScoringSettingUpdateDto(
    Guid Id,
    double Value);

public class UpdateScoringSettingsValidator : AbstractValidator<UpdateScoringSettingsCommand>
{}

    public UpdateScoringSettingsValidator()
    {
        RuleFor(x => x.Settings)
            .NotEmpty().WithMessage("Güncellenecek ayar listesi boş olamaz.");

        RuleForEach(x => x.Settings).ChildRules(item =>
        {
            item.RuleFor(i => i.Id)
                .NotEmpty().WithMessage("Ayar ID'si geçersiz.");

            item.RuleFor(i => i.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Katsayı değeri 0'dan küçük olamaz.");
        });
    }
}