// ─────────────────────────────────────────────────────────────
// ActivityQuestion Events
// ─────────────────────────────────────────────────────────────

// ─────────────────────────────────────────────────────────────
// ActivityQuestion Events
// ─────────────────────────────────────────────────────────────


namespace IzTek.Carbon.Footprint.Domain.Events.Activity;

public class ActivityQuestionCreatedDomainEvent(Guid id, string text, TimeSpan scheduledTime) : BaseEvent
{
    public Guid Id { get; init; } = id;
    public string Text { get; init; } = text;
    public TimeSpan ScheduledTime { get; init; } = scheduledTime;
}

public class ActivityQuestionUpdatedDomainEvent : BaseEvent
{
    public Guid Id { get; init; }
    public string Text { get; init; } = default!;
    public TimeSpan ScheduledTime { get; init; }

    public ActivityQuestionUpdatedDomainEvent(Guid id, string text, TimeSpan scheduledTime)
    {
        Id = id;
        Text = text;
        ScheduledTime = scheduledTime; // ✅ düzeltildi: NextQuestionId = NextQuestionId hatası kaldırıldı
    }
}

public class ActivityQuestionDeletedDomainEvent(Guid id) : BaseEvent
{
    public Guid Id { get; init; } = id;
}