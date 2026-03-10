namespace Iztek.Carbon.Footprint.Domain.Entities;

public class UserActivityAnswer
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid SelectedOptionId { get; private set; }
    public DateTime AnsweredAt { get; private set; }

    private UserActivityAnswer() { }

    public UserActivityAnswer(Guid userId, Guid questionId, Guid selectedOptionId, DateTime answeredAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
        AnsweredAt = answeredAt;
    }
}