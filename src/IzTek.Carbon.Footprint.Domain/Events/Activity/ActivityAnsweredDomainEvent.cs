namespace IzTek.Carbon.Footprint.Domain.Events.Activity;

/// <summary>
/// Kullanıcı günlük aktivite sorusunu cevapladığında fırlatılır.
/// → Takvim cache invalidation
/// → Günlük puan güncelleme
/// → Pending soru sayısı güncelleme
/// </summary>
public class ActivityAnsweredDomainEvent : BaseEvent
{
    public Guid UserId { get; init; }
    public Guid QuestionId { get; init; }
    public Guid SelectedOptionId { get; init; }
    public double CarbonValue { get; init; }
    public DateTime AnsweredAt { get; init; }
    public bool IsFlowCompleted { get; set; }

    public ActivityAnsweredDomainEvent(
        Guid userId,
        Guid questionId,
        Guid selectedOptionId,
        double carbonValue,
        DateTime answeredAt)
    {
        UserId = userId;
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
        CarbonValue = carbonValue;
        AnsweredAt = answeredAt;
    }
}