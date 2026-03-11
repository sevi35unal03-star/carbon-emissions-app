

namespace IzTek.Carbon.Footprint.Domain.Events.User;

/// <summary>
/// Event triggered when a user's carbon score needs to be updated.
/// </summary>
public class UserScoreUpdatedDomainEvent : BaseEvent // ✅ record → class
{
    public string UserId { get; init; } = default!;
    public double Score { get; init; }

    public UserScoreUpdatedDomainEvent(string userId, double score)
    {
        UserId = userId;
        Score = score;
    }
}