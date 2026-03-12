namespace IzTek.Carbon.Footprint.Domain.Events.Activity;

public class ActivityQuestionCreatedDomainEvent(Guid id, string text, TimeSpan scheduledTime) : BaseEvent
{
    public Guid Id { get; init; } = id;
    public string Text { get; init; } = text;
    public TimeSpan ScheduledTime { get; init; } = scheduledTime;
}

public class ActivityQuestionUpdatedDomainEvent(Guid id, string text, TimeSpan scheduledTime) : BaseEvent
{
    public Guid Id { get; init; } = id;
    public string Text { get; init; } = text;
    public TimeSpan ScheduledTime { get; init; } = scheduledTime;
}

public class ActivityQuestionDeletedDomainEvent(Guid id) : BaseEvent
{
    public Guid Id { get; init; } = id;
}