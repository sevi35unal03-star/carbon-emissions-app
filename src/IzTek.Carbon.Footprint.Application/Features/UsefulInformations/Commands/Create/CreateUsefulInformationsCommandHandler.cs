using Microsoft.Extensions.Caching.Memory;

namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Create;

public static class CreateUsefulInformationsCommandHandler
{
    public static async Task<Result> Handle(
        CreateUsefulInformationsCommand command,
        IApplicationDbContext context,
        IMemoryCache cache,
        CancellationToken ct)
    {
        var isExists = await context.UsefulInformations
            .AnyAsync(x => x.Title == command.Title, ct);

        if (isExists)
            return Result.Failure(SystemErrorCodes.InformationAlreadyExists, HttpStatusCode.BadRequest);

        var info = new UsefulInformation(
            command.Title,
            command.Content,
            command.DisplayOrder);

        await context.UsefulInformations.AddAsync(info, ct);

        var saved = await context.SaveChangesAsync(ct) > 0;

        if (saved)
            cache.Remove("usefulinformations");

        return saved
            ? Result.Created()
            : Result.SystemException();
    }
}