using System;
using System.Collections.Generic;
using System.Text;

namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Create
{
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



}
