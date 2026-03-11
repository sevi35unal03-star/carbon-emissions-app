

namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Update;

public class UpdateUsefulInformationsCommand
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public int DisplayOrder { get; set; }
}


public class CreateUpdateUsefulInformationCommandValidator : AbstractValidator<UpdateUsefulInformationsCommand>
{
    public CreateUpdateUsefulInformationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");
    }
}