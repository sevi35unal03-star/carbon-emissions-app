using IzTek.Carbon.Footprint.Domain.Events.Activity;

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class UserActivityAnswer : BaseAuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid SelectedOptionId { get; private set; }
    public double CarbonValue { get; private set; }
    public DateTime AnsweredAt { get; private set; }

    private UserActivityAnswer() { }

    public UserActivityAnswer(Guid userId, Guid questionId, Guid selectedOptionId, double carbonValue, DateTime answeredAt)
    {
        UserId = userId;
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
        CarbonValue = carbonValue;
        AnsweredAt = answeredAt;

        AddDomainEvent(new ActivityAnsweredDomainEvent(
            userId,
            questionId,
            selectedOptionId,
            carbonValue,
            answeredAt));
    }
}