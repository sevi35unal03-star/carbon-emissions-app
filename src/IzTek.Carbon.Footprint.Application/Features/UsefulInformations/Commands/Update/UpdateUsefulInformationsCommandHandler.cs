using Microsoft.Extensions.Caching.Memory;

namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Update;

public static class UpdateUsefulInformationCommandHandler
{
    public static async Task<Result> Handle(
        UpdateUsefulInformationsCommand command,
        IApplicationDbContext context,
        IMemoryCache cache,
        CancellationToken cancellationToken)
    {
        var info = await context.UsefulInformations
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (info is null)
            return Result.Failure(SystemErrorCodes.NotFound, HttpStatusCode.NotFound);

        var isTitleExists = await context.UsefulInformations
            .AnyAsync(x => x.Title == command.Title && x.Id != command.Id, cancellationToken);

        if (isTitleExists)
            return Result.Failure(SystemErrorCodes.InformationAlreadyExists, HttpStatusCode.BadRequest);

        info.Update(command.Title, command.Content, command.DisplayOrder);

        var success = await context.SaveChangesAsync(cancellationToken) > 0;

        if (success)
            cache.Remove("usefulinformations");

        return success
            ? Result.Success()
            : Result.Failure(SystemErrorCodes.UpdateFailed, HttpStatusCode.InternalServerError);
    }
}