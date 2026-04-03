using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Infrastructure.Services;

/// <summary>
/// Development ortamında platform API'si erişilemez olduğunda kullanılır.
/// Gerçek istek atmaz — her zaman başarılı döner.
/// </summary>
public class MockPlatformService(ILogger<MockPlatformService> logger) : IPlatformService
{
    public Task<Result?> SendEmailAsync(string to, string subject, string content)
    {
        logger.LogInformation("[MOCK] E-posta gönderildi → To: {To}, Subject: {Subject}", to, subject);
        return Task.FromResult<Result?>(Result.Success());
    }

    public Task<Result?> SendPushNotificationAsync(string target, string title, string body)
    {
        logger.LogInformation("[MOCK] Push gönderildi → Target: {Target}, Title: {Title}", target, title);
        return Task.FromResult<Result?>(Result.Success());
    }

    public Task<Result?> SendPushToAllUsersAsync(string title, string body, object? data = null)
    {
        logger.LogInformation("[MOCK] Push (all) gönderildi → Title: {Title}", title);
        return Task.FromResult<Result?>(Result.Success());
    }

    public Task<Result?> SendPushToUserAsync(string userId, string title, string body, object? data = null)
    {
        logger.LogInformation("[MOCK] Push (user) gönderildi → UserId: {UserId}, Title: {Title}", userId, title);
        return Task.FromResult<Result?>(Result.Success());
    }

    public Task<Result?> RegisterUserAsync(string email, string name, string surname, string phoneNumber)
    {
        logger.LogInformation("[MOCK] RegisterUser: {Email}", email);
        return Task.FromResult<Result?>(Result.Success());
    }

    public Task<Result?> ValidateUserAsync(string bizIzmirToken)
    {
        logger.LogInformation("[MOCK] ValidateUser: {Token}", bizIzmirToken);
        return Task.FromResult<Result?>(Result.Success());
    }
}