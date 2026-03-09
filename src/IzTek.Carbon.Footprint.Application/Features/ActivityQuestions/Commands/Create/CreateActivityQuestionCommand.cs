using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Create;

namespace Iztek.Carbon.Footprint.Application.Features.AcitivityQuestions.Commands.Create;

public class CreateActivityQuestionCommand
{
    public string Text { get; set; } = default!;
    public int DisplayOrder { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan ScheduledTime { get; set; }
    public TimeSpan NotificationTime { get; set; }
    public List<CreateActivityOptionRequest> Options { get; set; } = new();
}

public class CreateActivityQuestionCommandValidator : AbstractValidator<CreateActivityQuestionCommand>
{
    public CreateActivityQuestionCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Question text is required.")
            .MinimumLength(5).WithMessage("Question text must be at least 5 characters.")
            .MaximumLength(1000).WithMessage("Question text cannot exceed 1000 characters.");

        RuleFor(x => x.ScheduledTime)
            .NotEmpty().WithMessage("A scheduled time for notification is required.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be zero or positive.");

        RuleFor(x => x.Options)
            .NotEmpty().WithMessage("A question must have at least one option.")
            .Must(x => x != null && x.Count >= 1).WithMessage("At least one option is required.");

        RuleForEach(x => x.Options)
            .SetValidator(new CreateActivityOptionRequestValidator());
    }
}


