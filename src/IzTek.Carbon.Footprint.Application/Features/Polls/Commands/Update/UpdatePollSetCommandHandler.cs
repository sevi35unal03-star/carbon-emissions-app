namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Update;

public static class UpdatePollSetCommandHandler
{
    public static  async Task<Result> HandleAsync(
        UpdatePollSetCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. PollSet'i getir
        var pollSet = await context.PollSets
            .FirstOrDefaultAsync(x => x.Id == command.PollSetId, ct); // ✅ PollQuestionId → Id

        if (pollSet is null)
            return Result.Failure(SystemErrorCodes.PollSetNotFound, HttpStatusCode.NotFound); 

        // 2. Domain metodu ile güncelle
        pollSet.UpdateDetails(
            name: command.Name,
            description: command.Description,
            displayOrder: command.DisplayOrder); // ✅ Direkt property atama yerine domain metodu

        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}