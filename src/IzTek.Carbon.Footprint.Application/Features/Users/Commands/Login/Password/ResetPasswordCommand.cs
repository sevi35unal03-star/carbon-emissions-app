using IzTek.Carbon.Footprint.Application.Common.Validators;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login.Password;

public class ResetPasswordCommand
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string ResetCode { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
}