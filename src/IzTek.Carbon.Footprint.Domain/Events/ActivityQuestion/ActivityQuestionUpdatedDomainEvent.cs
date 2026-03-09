using IzTek.Carbon.Footprint.Domain.Common;

namespace IzTek.Carbon.Footprint.Domain.Events;

/// <summary>
/// Event triggered when an existing activity question is updated.
/// Crucial for re-scheduling notifications if the ScheduledTime has changed.
/// </summary>
public class ActivityQuestionUpdatedDomainEvent : BaseEvent
{
    public Guid Id { get; init; }
    public string Text { get; init; } = default!;
    public double CarbonValue { get; init; }
    public Guid NextQuestionId { get; init; }
    public TimeSpan ScheduledTime { get; init; }

    public ActivityQuestionUpdatedDomainEvent(Guid id, string text,double carbonValue, Guid nextQuestionId, TimeSpan scheduledTime)
    {
        Id = id;
        Text = text;
        CarbonValue = carbonValue;
        NextQuestionId = NextQuestionId;
        ScheduledTime = scheduledTime;
    }

    public ActivityQuestionUpdatedDomainEvent(Guid id)
    {
        Id = id;
    }
}