using IzTek.Carbon.Footprint.Domain.Events.Poll;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Notifications.Create;

public  class PollSetCreatedDomainEventHandler
{
    public static Task Handle(PollSetCreatedDomainEvent @event, ILogger<PollSetCreatedDomainEventHandler> logger)
    {
        logger.LogInformation("PollSet created with Id: {PollSetId}", @event.PollSetId);

        return Task.CompletedTask;
    }
}