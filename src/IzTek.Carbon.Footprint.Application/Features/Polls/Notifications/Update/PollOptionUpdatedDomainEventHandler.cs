using IzTek.Carbon.Footprint.Domain.Events.Poll;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Notifications.Update;

public record class PollOptionUpdatedDomainEventHandler
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