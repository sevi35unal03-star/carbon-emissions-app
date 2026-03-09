using Iztek.Carbon.Footprint.Domain.Entities;
using IzTek.Carbon.Footprint.Domain.Events.User;

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class UserActivityLog : BaseAuditableEntity 
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid ActivityQuestionId { get; private set; }
    public ActivityQuestion ActivityQuestion { get; private set; } = null!;

    public Guid ActivityOptionId { get; private set; }
    public ActivityOption ActivityOption { get; private set; } = null!;

    public double TotalCarbonScore { get; private set; }
    public DateTime ActivityDate { get; private set; }
    public string SelectedOptionText { get; private set; } = null!;

    public string CarbonValue { get; private set; } = null!;


    private UserActivityLog() { }

    public UserActivityLog(Guid userId, Guid questionId, Guid optionId, double score)
    {
        UserId = userId;
        ActivityQuestionId = questionId;
        ActivityOptionId = optionId;
        TotalCarbonScore = score;
        ActivityDate = DateTime.UtcNow;

        AddDomainEvent(new UserScoreUpdatedDomainEvent(userId.ToString(), score));
    }
}