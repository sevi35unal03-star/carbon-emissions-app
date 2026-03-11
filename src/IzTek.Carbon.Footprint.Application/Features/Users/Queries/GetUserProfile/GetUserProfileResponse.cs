namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileResponse
{
    public GetUserProfileResponse(string v, string? name, string? surname, DateTime? birthDate)
    {
        Name = name; 
        Surname = surname;
        BirthDate = birthDate;
    }

    public string IdentityNumber {  get; set; }
     public string Name { get; set; } 
    public string Surname { get; set; }
    public DateTime? BirthDate { get; set; }

}
  