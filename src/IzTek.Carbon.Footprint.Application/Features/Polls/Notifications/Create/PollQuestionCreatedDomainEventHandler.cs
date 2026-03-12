using IzTek.Carbon.Footprint.Domain.Events.Poll;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Notifications.Create;

public record class PollQuestionCreatedDomainEventHandler
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