
namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Delete;

public static class DeletePollOptionCommandHandler
{
    public static async Task<Result> Handle(
    DeletePollOptionCommand command,
    IApplicationDbContext context,
    IMessageBus bus,
    CancellationToken ct)
    {
        var option = await context.PollOptions
            .FirstOrDefaultAsync(x => x.Id == command.OptionId, ct);

        if (option == null)
            return Result.Failure(
                SystemErrorCodes.PollOptionNotFound, HttpStatusCode.NotFound);

        context.PollOptions.Remove(option);

        return await context.SaveChangesAsync(ct) > 0
            ? Result.NoContent()
            : Result.SystemException();
    }
}