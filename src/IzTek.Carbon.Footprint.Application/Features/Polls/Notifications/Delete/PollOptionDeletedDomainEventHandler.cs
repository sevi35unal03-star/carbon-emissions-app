using IzTek.Carbon.Footprint.Domain.Events.Poll;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Notifications.Delete;

public record class PollOptionDeletedDomainEventHandler
{
    public static Task Handle(PollOptionDeletedDomainEvent @event, ILogger<PollOptionDeletedDomainEventHandler> logger)
    {
        logger.LogInformation("PollOption deleted with Id: {PollSetId}", @event.PollOptionId);

        return Task.CompletedTask;
    }
}