

namespace IzTek.Carbon.Footprint.Domain.Events.Poll;

/// <summary>
/// Kullanıcı aylık anketi tamamladığında fırlatılır.
/// → Liderboard cache invalidation
/// → Hedef (Goal) tamamlanma kontrolü
/// → Kullanıcı toplam puan güncelleme
/// </summary>
public class PollAnsweredDomainEvent : BaseEvent
{
    public Guid UserId { get; init; }
    public Guid PollSetId { get; init; }
    public double TotalScore { get; init; }
    public int TreeCount { get; init; }
    public int Month { get; init; }
    public int Year { get; init; }

    public PollAnsweredDomainEvent(
        Guid userId,
        Guid pollSetId,
        double totalScore,
        int treeCount,
        int month,
        int year)
    {
        UserId = userId;
        PollSetId = pollSetId;
        TotalScore = totalScore;
        TreeCount = treeCount;
        Month = month;
        Year = year;
    }
}