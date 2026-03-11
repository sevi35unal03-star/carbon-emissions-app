namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;

public class UserDailyResultResponse
{
    public Guid Id { get; set; }
    public DateTime? LastLoginDate { get; set; }  
    public double CarbonFootprintScore { get; set; }
    public int DailyActivitiesCount { get; set; }
    public double TotalCurrentScore { get; set; }
    public int DonatedTreeCount { get; set; }
    public double EquivalentPoints { get; set; }
    }

