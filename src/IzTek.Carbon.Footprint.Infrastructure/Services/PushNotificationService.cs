namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class PushNotificationService : IPushNotificationService
{
    public async Task SendToUserAsync(Guid userId, string title, string body, CancellationToken ct = default)
    {
        // TODO: Firebase FCM entegrasyonu yapılacak - fcm cihaz idsi
        await Task.CompletedTask;
    }

    public async Task SendToAdminAsync(string title, string body, CancellationToken ct = default)
    {
        // TODO: Firebase FCM entegrasyonu yapılacak
        await Task.CompletedTask;
    }
}