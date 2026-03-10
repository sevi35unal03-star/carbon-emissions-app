namespace IzTek.Carbon.Footprint.Domain.Entities;

public class TreeDonation : BaseAuditableEntity
{
    public Guid UserId { get; private set; }
    public int TreeCount { get; private set; }
    public double PointsSpent { get; private set; }
    public DateTime DonationDate { get; private set; }

    private TreeDonation() { }

    public TreeDonation(Guid userId, int treeCount, double pointsSpent)
    {
        UserId = userId;
        TreeCount = treeCount;
        PointsSpent = pointsSpent;
        DonationDate = DateTime.UtcNow;
    }
}