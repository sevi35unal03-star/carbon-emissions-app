using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Events;

public class PollSetDeletedDomainEventHandler
{
    public static Task Handle(PollSetDeletedDomainEvent @event, ILogger<PollSetDeletedDomainEventHandler> logger)
    {
        logger.LogInformation("PollSet deleted with Id: {PollSetId}", @event.PollSetId);

        return Task.CompletedTask;
    }
}