using IzTek.Carbon.Footprint.Domain.Events.Poll;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Notifications.Update;

public class PollQuestionUpdatedDomainEventHandler
{
    public static Task Handle(
        PollQuestionUpdatedDomainEvent @event,
        ILogger<PollQuestionUpdatedDomainEventHandler> logger)
    {
        logger.LogInformation(
            "PollQuestion updated. QuestionId: {QuestionId}",
            @event.PollQuestionId);

        return Task.CompletedTask;
    }
}