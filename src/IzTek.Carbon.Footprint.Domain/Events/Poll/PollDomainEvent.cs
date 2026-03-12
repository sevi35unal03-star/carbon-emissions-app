namespace IzTek.Carbon.Footprint.Domain.Events.Poll;

public class PollSetCreatedDomainEvent(Guid pollSetId, string name) : BaseEvent
{
    public Guid PollSetId { get; init; } = pollSetId;
    public string Name { get; init; } = name;
}

public class PollSetUpdatedDomainEvent(Guid pollSetId, string name) : BaseEvent
{
    public Guid PollSetId { get; init; } = pollSetId;
    public string Name { get; init; } = name;
}

public class PollSetDeletedDomainEvent(Guid pollSetId) : BaseEvent
{
    public Guid PollSetId { get; init; } = pollSetId;
}

public class PollQuestionCreatedDomainEvent(Guid pollQuestionId, Guid pollSetId) : BaseEvent
{
    public Guid PollQuestionId { get; init; } = pollQuestionId;
    public Guid PollSetId { get; init; } = pollSetId;
}

public class PollQuestionUpdatedDomainEvent(Guid pollQuestionId) : BaseEvent
{
    public Guid PollQuestionId { get; init; } = pollQuestionId;
}

public class PollQuestionDeletedDomainEvent(Guid pollQuestionId) : BaseEvent
{
    public Guid PollQuestionId { get; init; } = pollQuestionId;
}

public class PollOptionCreatedDomainEvent(Guid pollOptionId, Guid pollQuestionId) : BaseEvent
{
    public Guid PollOptionId { get; init; } = pollOptionId;
    public Guid PollQuestionId { get; init; } = pollQuestionId;
}

public class PollOptionUpdatedDomainEvent(Guid pollOptionId) : BaseEvent
{
    public Guid PollOptionId { get; init; } = pollOptionId;
}

public class PollOptionDeletedDomainEvent(Guid pollOptionId) : BaseEvent
{
    public Guid PollOptionId { get; init; } = pollOptionId;
}