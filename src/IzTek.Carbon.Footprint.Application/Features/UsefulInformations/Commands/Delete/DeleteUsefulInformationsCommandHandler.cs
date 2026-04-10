using Microsoft.Extensions.Caching.Memory;

namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Delete;

public static class DeleteUsefulInformationsCommandHandler
{
    public static async Task<Result> Handle(
        DeleteUsefulInformationsCommand command,
        IApplicationDbContext context,
        IMemoryCache cache,
        CancellationToken ct)
    {
        var info = await context.UsefulInformations
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (info is null)
            return Result.Failure(SystemErrorCodes.NotFound, HttpStatusCode.NotFound);

        info.IsDeleted = true;
        info.DeletedAt = DateTime.UtcNow;

        context.UsefulInformations.Remove(info);

        var success = await context.SaveChangesAsync(ct) > 0;

        if (success)
            cache.Remove("usefulinformations");

        return success
            ? Result.NoContent()
            : Result.Failure(SystemErrorCodes.DeleteFailed, HttpStatusCode.InternalServerError);
    }
}