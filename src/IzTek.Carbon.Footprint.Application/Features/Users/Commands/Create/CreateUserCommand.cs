using IzTek.Carbon.Footprint.Application.Common.Validators;
using IzTek.Carbon.Footprint.Domain.Common;


namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Create;

public class CreateUserCommand
{
    public string FirstName { get; init; } = string.Empty;       
    public string LastName { get; init; } = string.Empty;
    public string IdentityNumber { get; init; } = string.Empty;
    public string PhoneNumber { get; set; } = null!;
    public DateTime BirthDate { get; init; }
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsKvkkApproved { get; init; }
}

