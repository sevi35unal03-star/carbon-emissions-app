namespace Iztek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Create;

public static class CreateUsefulInformationsCommandHandler
{
    public static async Task<Result> HandleAsync(
        CreateUsefulInformationsCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var isExists = await context.UsefulInformations
            .AnyAsync(x => x.Title == command.Title, ct);

        if (isExists)
        {
            return Result.Failure("InformationAlreadyExists", HttpStatusCode.BadRequest);
        }

        var info = new UsefulInformation(
            command.Title,
            command.Content,
            command.DisplayOrder
            );

        await context.UsefulInformations.AddAsync(info, ct);

        return await context.SaveChangesAsync(ct) > 0
            ? Result.Created()
            : Result.Failure("SystemException", HttpStatusCode.InternalServerError);
    }
}