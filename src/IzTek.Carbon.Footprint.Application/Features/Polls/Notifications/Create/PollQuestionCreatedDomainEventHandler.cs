using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Events;

public class PollQuestionCreatedDomainEventHandler
{
    public static Task Handle(
        PollQuestionCreatedDomainEvent @event,
        ILogger<PollQuestionCreatedDomainEventHandler> logger)
    {
        logger.LogInformation(
            "PollQuestion created. QuestionId: {QuestionId}, PollSetId: {PollSetId}",
            @event.PollQuestionId,
            @event.PollSetId);

        return Task.CompletedTask;
    }
}