using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Features.Polls.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Delete;

public class DeletePollOptionCommandHandler
{
    public static async Task Handle(
        DeletePollOptionCommand command,
        IApplicationDbContext context,
        IMessageBus bus,
        CancellationToken ct)
    {
        var option = await context.PollOptions
            .FirstOrDefaultAsync(x => x.PollQuestionId == command.OptionId, ct);

        if (option == null)
            throw new Exception("Option not found.");

        context.PollOptions.Remove(option);

        await context.SaveChangesAsync(ct);

        await bus.PublishAsync(new PollOptionDeletedDomainEvent(command.OptionId), ct);
    }
}