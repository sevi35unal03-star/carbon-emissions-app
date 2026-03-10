namespace Iztek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;

public class GetUsersDetailedResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty;
    public double TotalCarbonScore { get; set; }
    public bool IsKvkkApproved { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
};

