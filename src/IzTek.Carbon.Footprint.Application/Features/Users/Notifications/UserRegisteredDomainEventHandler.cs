using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Notifications;

public class UserRegisteredEventHandler(
    ILogger<UserRegisteredEventHandler> logger)
{
    public Task Handle(
        UserRegisteredDomainEvent notification,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Yeni kullanıcı kaydı → UserId: {UserId}, Ad Soyad: {FullName}, Email: {Email}, Tarih: {Date}",
            notification.UserId,
            notification.FullName,
            notification.Email,
            DateTime.UtcNow);

        return Task.CompletedTask;
    }
}