using IzTek.Carbon.Footprint.Domain.Common;

namespace IzTek.Carbon.Footprint.Domain.Events;

/// <summary>
/// Event triggered when a new activity question is created.
/// Used for scheduling notifications and cache invalidation.
/// </summary>
public class ActivityQuestionCreatedDomainEvent : BaseEvent
{
    public Guid Id { get; init; }
    public string Text { get; init; } = default!;
    public TimeSpan ScheduledTime { get; init; }

    public ActivityQuestionCreatedDomainEvent(Guid id, string text, TimeSpan scheduledTime)
    {
        Id = id;
        Text = text;
        ScheduledTime = scheduledTime;
    }

    public ActivityQuestionCreatedDomainEvent(Guid id, TimeSpan scheduledTime)
    {
        Id = id;
        ScheduledTime = scheduledTime;
    }
}


