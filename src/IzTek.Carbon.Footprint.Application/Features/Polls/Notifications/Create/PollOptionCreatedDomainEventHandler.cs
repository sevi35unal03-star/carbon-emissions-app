using IzTek.Carbon.Footprint.Domain.Events.Poll;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Notifications.Create;

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