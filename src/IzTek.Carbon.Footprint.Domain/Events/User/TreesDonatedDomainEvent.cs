

namespace IzTek.Carbon.Footprint.Domain.Events.User;

/// <summary>
/// Kullanıcı ağaç bağışı yaptığında fırlatılır.
/// → DonatedTreeCount güncelleme
/// → TotalPoints sıfırlama
/// → Bağış bildirimi
/// </summary>
public class TreesDonatedDomainEvent(
    Guid userId,
    int treeCount,
    double pointsSpent,
    DateTime donationDate) : BaseEvent
{
    public Guid UserId { get; init; } = userId;
    public int TreeCount { get; init; } = treeCount;
    public double PointsSpent { get; init; } = pointsSpent;
    public DateTime DonationDate { get; init; } = donationDate;
}