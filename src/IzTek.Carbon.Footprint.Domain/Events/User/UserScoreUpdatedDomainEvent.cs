using IzTek.Carbon.Footprint.Domain.Common;

namespace IzTek.Carbon.Footprint.Domain.Events.User;

/// <summary>
/// Event triggered when a user's carbon score needs to be updated.
/// </summary>
public record UserScoreUpdatedDomainEvent : BaseEvent
{
    public string UserId { get; init; }
    public double Score { get; init; }

    public UserScoreUpdatedDomainEvent(string userId, double score)
    {
        UserId = userId;
        Score = score;
    }
}