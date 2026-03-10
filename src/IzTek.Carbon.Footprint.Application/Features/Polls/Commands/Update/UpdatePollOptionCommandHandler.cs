namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Update;

public class UpdatePollOptionCommandHandler
{
    public async Task<Result> HandleAsync(
        UpdatePollOptionCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Option'ı getir
        var option = await context.PollOptions
            .FirstOrDefaultAsync(x => x.Id == command.OptionId, ct); // ✅ PollQuestionId → Id

        if (option is null)
            return Result.Failure(SystemErrorCodes.PollOptionNotFound, HttpStatusCode.NotFound); // ✅ throw yerine Result.Failure

        // 2. Domain metodu ile güncelle
        option.UpdateDetails(
            text: command.Text,
            carbonValue: command.Value,
            nextPollQuestionId: command.NextPollQuestionId,
            displayOrder: command.DisplayOrder); // ✅ Direkt property atama yerine domain metodu

        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}