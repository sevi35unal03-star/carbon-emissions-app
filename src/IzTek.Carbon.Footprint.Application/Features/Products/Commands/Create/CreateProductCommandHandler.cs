namespace IzTek.Carbon.Footprint.Application.Features.Products.Commands.Create;

public static class CreateProductCommandHandler
{
    public static async Task<Result> HandleAsync(
        CreateProductCommand command,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var isExists = await context.Products.Where(x => x.Name == command.Name).AnyAsync(cancellationToken);

        if (isExists)
        {
            return Result.Failure(SystemErrorCodes.ProductAlreadyExists, System.Net.HttpStatusCode.BadRequest);
        }

        var product = new Product(command.Name, command.Category);

        await context.Products.AddAsync(product, cancellationToken);

        product.AddDomainEvent(new ProductCreatedDomainEvent()
        {
            Id = product.PollQuestionId,
            Name = product.Name,
        });

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? Result.Created()
            : Result.SystemException();
    }
}