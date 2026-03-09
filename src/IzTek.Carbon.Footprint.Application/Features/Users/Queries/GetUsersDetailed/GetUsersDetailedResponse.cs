namespace Iztek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;

public class GetUsersDetailedResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string IdentityNumber { get; set; }
    public double TotalCarbonScore { get; set; }
    public bool IsKvkkApproved { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
};

