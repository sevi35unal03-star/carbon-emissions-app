namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public static class CreatePollSetCommandHandler
{
    public static async Task<Result<Guid>> Handle(
        CreatePollSetCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. PollSet oluştur
        var pollSet = new PollSet(
            name: command.Name,
            description: command.Description ?? string.Empty,
            displayOrder: command.DisplayOrder,
            month: command.Month,
            year: command.Year
        );

        context.PollSets.Add(pollSet);
        await context.SaveChangesAsync(ct);

        return Result<Guid>.Success(pollSet.Id); 
    }
}