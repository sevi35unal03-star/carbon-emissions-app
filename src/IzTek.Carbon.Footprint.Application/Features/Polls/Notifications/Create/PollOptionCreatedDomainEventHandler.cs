using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Events;

public class PollOptionCreatedDomainEventHandler
{
    public static Task Handle(
        PollOptionCreatedDomainEvent @event,
        ILogger<PollOptionCreatedDomainEventHandler> logger)
    {
        logger.LogInformation(
            "PollOption created. OptionId: {OptionId}, QuestionId: {QuestionId}",
            @event.PollOptionId,
            @event.PollQuestionId);

        return Task.CompletedTask;
    }
}