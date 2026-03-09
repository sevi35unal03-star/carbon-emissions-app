namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Create;

public class CreateActivityOptionRequest
{
    public string Text { get; set; } = default!;
    public double CarbonValue { get; set; }
    public Guid? NextQuestionId { get; set; }
}

public class CreateActivityOptionRequestValidator : AbstractValidator<CreateActivityOptionRequest>
{
    public CreateActivityOptionRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Option text cannot be empty.")
            .MaximumLength(250).WithMessage("Option text cannot exceed 250 characters.");

        RuleFor(x => x.CarbonValue)
            .NotNull().WithMessage("Carbon value must be defined.");
    }
}