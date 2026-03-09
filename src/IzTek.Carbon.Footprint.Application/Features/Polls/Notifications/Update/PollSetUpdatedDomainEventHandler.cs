using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Events;

public class PollSetUpdatedDomainEventHandler
{
    public static Task Handle(PollSetUpdatedDomainEvent @event, ILogger<PollSetUpdatedDomainEventHandler> logger)
    {
        logger.LogInformation("PollSet updated with Id: {PollSetId}", @event.PollSetId);

        return Task.CompletedTask;
    }
}