namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileResponse(
    string identityNumber,
    string? name,
    string? surname,
    DateTime? birthDate,
    double totalPoints,
    int donatedTreeCount,
    int availableTreeCount)   // ← Bağışlanabilecek ağaç sayısı
{
    public string IdentityNumber { get; set; } = identityNumber;
    public string? Name { get; set; } = name;
    public string? Surname { get; set; } = surname;
    public DateTime? BirthDate { get; set; } = birthDate;
    public double TotalPoints { get; set; } = totalPoints;
    public int DonatedTreeCount { get; set; } = donatedTreeCount;
    public int AvailableTreeCount { get; set; } = availableTreeCount; // Buton: "Ağaç Bağışla 1200 Ağaç"
}