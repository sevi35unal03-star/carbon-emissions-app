namespace IzTek.Carbon.Footprint.Application.Features.Roles.Notifications;

public static class RoleUpdatedDomainEventHandler
{
    public static async Task HandleAsync(
        RoleUpdatedDomainEvent @event,
        IPlatformService platformService,
        ILogger<RoleUpdatedDomainEvent> logger,
        CancellationToken cancellationToken)
    {
        // 1. İşlem Başlangıcı Logu
        logger.LogInformation("Processing role update notification. Role ID: {RoleId}, New Name: {RoleName}, New Type: {RoleType}",
            @event.Id, @event.Name, @event.Type);

        try
        {
            // 2. İş Mantığı: Bildirim içeriğini hazırla
            var message = $"Role update notification: The role with ID {@event.Id} has been updated. New configuration: {@event.Name} (Type: {@event.Type})";

            // 3. Eylem: Yöneticiye mail gönder (PlatformService aracılığıyla)
            await platformService.SendEmailAsync("admin@iztek.com", message);

            // 4. Başarı Logu
            logger.LogInformation("Role update notification sent successfully for Role ID: {RoleId}", @event.Id);
        }
        catch (Exception ex)
        {
            // 5. Hata Logu
            logger.LogError(ex, "An error occurred while sending role update notification. Role ID: {RoleId}", @event.Id);
        }
    }
}