using IzTek.Carbon.Footprint.Domain.Events.Poll;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Notifications.Update;

public class PollSetUpdatedDomainEventHandler
{
    public static Task Handle(PollSetUpdatedDomainEvent @event, ILogger<PollSetUpdatedDomainEventHandler> logger)
    {
        logger.LogInformation("PollSet updated with Id: {PollSetId}", @event.PollSetId);

        return Task.CompletedTask;
    }
}