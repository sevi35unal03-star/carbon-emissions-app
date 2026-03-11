

namespace IzTek.Carbon.Footprint.Domain.Events.User;

/// <summary>
/// Kullanıcı ağaç bağışı yaptığında fırlatılır.
/// → DonatedTreeCount güncelleme
/// → TotalPoints sıfırlama
/// → Bağış bildirimi
/// </summary>
public class TreesDonatedDomainEvent : BaseEvent
{
    public Guid UserId { get; init; }
    public int TreeCount { get; init; }
    public double PointsSpent { get; init; }
    public DateTime DonationDate { get; init; }

    public TreesDonatedDomainEvent(
        Guid userId,
        int treeCount,
        double pointsSpent,
        DateTime donationDate)
    {
        UserId = userId;
        TreeCount = treeCount;
        PointsSpent = pointsSpent;
        DonationDate = donationDate;
    }
}