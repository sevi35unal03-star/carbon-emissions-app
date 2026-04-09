namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileResponse
{
    public string IdentityNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateTime? BirthDate { get; set; }
    public double TotalPoints { get; set; }
    public int DonatedTreeCount { get; set; }
    public int AvailableTreeCount { get; set; }   // "Ağaç Bağışla 1200 Ağaç" butonu için

    public GetUserProfileResponse(
        string identityNumber,
        string? name,
        string? surname,
        DateTime? birthDate,
        double totalPoints,
        int donatedTreeCount,
        int availableTreeCount)
    {
        IdentityNumber = identityNumber;
        Name = name;
        Surname = surname;
        BirthDate = birthDate;
        TotalPoints = totalPoints;
        DonatedTreeCount = donatedTreeCount;
        AvailableTreeCount = availableTreeCount;
    }
}