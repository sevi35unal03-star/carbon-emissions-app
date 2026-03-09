namespace IzTek.Carbon.Footprint.Application.Features.Polls.Events;

public record PollOptionCreatedDomainEvent(Guid PollOptionId, Guid PollQuestionId);