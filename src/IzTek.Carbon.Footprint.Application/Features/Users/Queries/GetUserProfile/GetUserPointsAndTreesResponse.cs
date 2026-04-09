namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

public class GetUserPointsAndTreesResponse
{
    public double TotalPoints { get; set; }
    public int DonatedTreeCount { get; set; }
    public int AvailableTreeCount { get; set; }

    public GetUserPointsAndTreesResponse(double totalPoints, int donatedTreeCount, int availableTreeCount)
    {
        TotalPoints = totalPoints;
        DonatedTreeCount = donatedTreeCount;
        AvailableTreeCount = availableTreeCount;
    }
}