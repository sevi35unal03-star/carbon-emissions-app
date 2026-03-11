// ─────────────────────────────────────────────────────────────
// Poll Events — Domain layer'a taşındı (Application'dan)

// ─────────────────────────────────────────────────────────────

namespace IzTek.Carbon.Footprint.Domain.Events.Poll;

public class PollSetCreatedDomainEvent : BaseEvent
{
    public Guid PollSetId { get; init; }
    public string Name { get; init; } = default!;

    public PollSetCreatedDomainEvent(Guid pollSetId, string name)
    {
        PollSetId = pollSetId;
        Name = name;
    }
}

public class PollSetUpdatedDomainEvent : BaseEvent
{
    public Guid PollSetId { get; init; }
    public string Name { get; init; } = default!;

    public PollSetUpdatedDomainEvent(Guid pollSetId, string name)
    {
        PollSetId = pollSetId;
        Name = name;
    }
}

public class PollSetDeletedDomainEvent : BaseEvent
{
    public Guid PollSetId { get; init; }

    public PollSetDeletedDomainEvent(Guid pollSetId)
    {
        PollSetId = pollSetId;
    }
}

public class PollQuestionCreatedDomainEvent : BaseEvent
{
    public Guid PollQuestionId { get; init; }
    public Guid PollSetId { get; init; }

    public PollQuestionCreatedDomainEvent(Guid pollQuestionId, Guid pollSetId)
    {
        PollQuestionId = pollQuestionId;
        PollSetId = pollSetId;
    }
}

public class PollQuestionUpdatedDomainEvent : BaseEvent
{
    public Guid PollQuestionId { get; init; }

    public PollQuestionUpdatedDomainEvent(Guid pollQuestionId)
    {
        PollQuestionId = pollQuestionId;
    }
}

public class PollQuestionDeletedDomainEvent : BaseEvent
{
    public Guid PollQuestionId { get; init; }

    public PollQuestionDeletedDomainEvent(Guid pollQuestionId)
    {
        PollQuestionId = pollQuestionId;
    }
}

public class PollOptionCreatedDomainEvent : BaseEvent
{
    public Guid PollOptionId { get; init; }
    public Guid PollQuestionId { get; init; }

    public PollOptionCreatedDomainEvent(Guid pollOptionId, Guid pollQuestionId)
    {
        PollOptionId = pollOptionId;
        PollQuestionId = pollQuestionId;
    }
}

public class PollOptionUpdatedDomainEvent : BaseEvent
{
    public Guid PollOptionId { get; init; }

    public PollOptionUpdatedDomainEvent(Guid pollOptionId)
    {
        PollOptionId = pollOptionId;
    }
}

public class PollOptionDeletedDomainEvent : BaseEvent
{
    public Guid PollOptionId { get; init; }

    public PollOptionDeletedDomainEvent(Guid pollOptionId)
    {
        PollOptionId = pollOptionId;
    }
}