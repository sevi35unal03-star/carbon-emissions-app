namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileResponse
{
    public string IdentityNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateTime? BirthDate { get; set; }

    // Constructor - sadece temel bilgileri alacak
    public GetUserProfileResponse(
        string identityNumber,
        string? name,
        string? surname,
        DateTime? birthDate)
    {
        IdentityNumber = identityNumber;
        Name = name;
        Surname = surname;
        BirthDate = birthDate;
    }
}