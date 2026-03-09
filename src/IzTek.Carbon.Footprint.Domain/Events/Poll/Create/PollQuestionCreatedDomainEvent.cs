namespace IzTek.Carbon.Footprint.Application.Features.Polls.Events;

public record PollQuestionCreatedDomainEvent(Guid PollQuestionId, Guid PollSetId);