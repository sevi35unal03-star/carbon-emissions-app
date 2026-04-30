using IzTek.Carbon.Footprint.Application.Common.Validators;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

public class ForgotPasswordCommand
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string DeviceToken { get; set; }

    public ForgotPasswordCommand(string deviceToken)
    {
        DeviceToken = deviceToken;
    }
}

