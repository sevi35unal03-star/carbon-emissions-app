namespace IzTek.Carbon.Footprint.Domain.Events.Poll.Create;

public record PollSetCreatedDomainEvent(Guid PollSetId, string name);