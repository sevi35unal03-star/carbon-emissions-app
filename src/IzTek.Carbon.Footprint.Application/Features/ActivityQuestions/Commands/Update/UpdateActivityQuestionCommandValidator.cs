namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Update
{
    public class UpdateActivityQuestionCommandValidator : AbstractValidator<UpdateActivityQuestionCommand>
    {
        public UpdateActivityQuestionCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.ScheduledTime).NotEmpty();
            RuleFor(x => x.Options).NotEmpty().Must(x => x.Count >= 2)
                .WithMessage("A question must have at least 2 options.");

            RuleForEach(x => x.Options).ChildRules(option =>
            {
                option.RuleFor(x => x.Text).NotEmpty().MaximumLength(250);
            });
        }
    }
}
