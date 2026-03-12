namespace IzTek.Carbon.Footprint.Domain.Events.Poll;

/// <summary>
/// Kullanıcı aylık anketi tamamladığında fırlatılır.
/// → Liderboard cache invalidation
/// → Hedef (Goal) tamamlanma kontrolü
/// → Kullanıcı toplam puan güncelleme
/// </summary>
public class PollAnsweredDomainEvent(
    Guid userId,
    Guid pollSetId,
    double totalScore,
    int treeCount,
    int month,
    int year) : BaseEvent
{
    public Guid UserId { get; init; } = userId;
    public Guid PollSetId { get; init; } = pollSetId;
    public double TotalScore { get; init; } = totalScore;
    public int TreeCount { get; init; } = treeCount;
    public int Month { get; init; } = month;
    public int Year { get; init; } = year;
}