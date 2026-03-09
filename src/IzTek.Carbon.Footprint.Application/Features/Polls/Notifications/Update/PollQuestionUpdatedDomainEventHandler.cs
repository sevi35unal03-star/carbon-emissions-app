using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Events;

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