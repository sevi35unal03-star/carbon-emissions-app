namespace IzTek.Carbon.Footprint.Application.Features.Products.Commands.Create;

public class CreateProductCommand
{
    public string Name { get; set; }
    public CategoryType Category { get; set; }
}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Category).IsInEnum();
    }
}