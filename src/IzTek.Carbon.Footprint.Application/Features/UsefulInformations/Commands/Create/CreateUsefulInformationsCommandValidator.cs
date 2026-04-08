namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Create
{
    public class CreateUsefulInformationsCommandValidator : AbstractValidator<CreateUsefulInformationsCommand>
    {
        public CreateUsefulInformationsCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");
        }
    }
}
