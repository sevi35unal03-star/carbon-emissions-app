namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;

public class LoginCommand
{
    public string EmailorIdentityNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

