namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileResponse( string identityNumber, string name, string surname, DateTime? birthDate)
{
    public string IdentityNumber { get; set; } = identityNumber;
    public string Name { get; set; } = name;
    public string Surname { get; set; } = surname;
    public DateTime? BirthDate { get; set; } = birthDate;

}
  