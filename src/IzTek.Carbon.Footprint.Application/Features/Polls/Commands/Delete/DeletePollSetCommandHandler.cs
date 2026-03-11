namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Delete;

public static class DeletePollSetCommandHandler
{
    public static async Task<Result> Handle(
    DeletePollSetCommand command,
    IApplicationDbContext context,
    IMessageBus bus,
    CancellationToken ct)
    {
        var pollSet = await context.PollSets
            .Include(x => x.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(x => x.Id == command.PollSetId, ct);

        if (pollSet == null)
            return Result.Failure(
                SystemErrorCodes.PollSetNotFound, HttpStatusCode.NotFound);

        context.PollSets.Remove(pollSet);

        if (await context.SaveChangesAsync(ct) <= 0)
            return Result.Failure(
                SystemErrorCodes.PollSetDeleteFailed, HttpStatusCode.InternalServerError);

        return Result.NoContent();
    }
}