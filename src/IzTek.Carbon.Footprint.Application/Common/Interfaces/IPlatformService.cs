namespace IzTek.Carbon.Footprint.Application.Common.Interfaces;

public interface IPlatformService
{
    Task<Result?> SendEmailAsync(string to, string subject, string content);

    Task<Result?> SendPushNotificationAsync(string target, string title, string body);

    Task<Result?> SendPushToAllUsersAsync(string title, string body, object? data = null); // ✅

    Task<Result?> SendPushToUserAsync(string userId, string title, string body, object? data = null); // ✅

    // BizIzmir entegrasyonu — ileride implement edilecek
    Task<Result?> RegisterUserAsync(string email, string name, string surname, string phoneNumber);

    Task<Result?> ValidateUserAsync(string bizIzmirToken);
}