using IzTek.Carbon.Footprint.Domain.Events.Poll;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Notifications.Delete;

public class PollQuestionDeletedDomainEventHandler
{
    public static Task Handle(
        PollQuestionDeletedDomainEvent @event,
        ILogger<PollQuestionDeletedDomainEventHandler> logger)
    {
        logger.LogInformation(
            "PollQuestion deleted. QuestionId: {QuestionId}",
            @event.PollQuestionId);

        return Task.CompletedTask;
    }
}