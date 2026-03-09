using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Events;

public class PollOptionUpdatedDomainEventHandler
{
    public static Task Handle(
        PollOptionUpdatedDomainEvent @event,
        ILogger<PollOptionUpdatedDomainEventHandler> logger)
    {
        logger.LogInformation(
            "PollOption updated. OptionId: {OptionId}",
            @event.PollOptionId);

        return Task.CompletedTask;
    }
}