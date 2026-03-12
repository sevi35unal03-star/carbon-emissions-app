namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;

public class GetUsersDetailedResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string IdentityNumber { get; set; } = null!;
    public double TotalCarbonScore { get; set; }
    public bool IsKvkkApproved { get; set; }
    public DateTime? BirthDate { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
};

