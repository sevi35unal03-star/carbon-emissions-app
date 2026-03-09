namespace IzTek.Carbon.Footprint.Application.Features.Polls.Events;

public record PollSetUpdatedDomainEvent(Guid PollSetId, string name);