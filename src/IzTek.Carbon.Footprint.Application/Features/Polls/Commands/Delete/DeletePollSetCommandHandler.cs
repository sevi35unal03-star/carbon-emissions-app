using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Features.Polls.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Delete;

public class DeletePollSetCommandHandler
{
    public static async Task Handle(
        DeletePollSetCommand command,
        IApplicationDbContext context,
        IMessageBus bus,
        CancellationToken ct)
    {
        var pollSet = await context.PollSets
            .Include(x => x.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(x => x.PollQuestionId == command.PollSetId, ct);

        if (pollSet == null)
            throw new Exception("PollSet not found.");

        context.PollSets.Remove(pollSet);

        await context.SaveChangesAsync(ct);

       // await bus.PublishAsync(new PollSetDeletedDomainEvent(command.PollSetId), ct);
    }
}