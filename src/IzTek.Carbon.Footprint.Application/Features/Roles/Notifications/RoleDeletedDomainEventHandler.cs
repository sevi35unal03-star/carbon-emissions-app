namespace IzTek.Carbon.Footprint.Application.Features.Roles.Notifications;

public static class RoleDeletedDomainEventHandler
{
    public static async Task HandleAsync(
        RoleDeletedDomainEvent @event,
        IPlatformService platformService,
        ILogger<RoleDeletedDomainEvent> logger,
        CancellationToken cancellationToken)
    {
        // 1. Log: Silme işleminin işlendiğini kaydet
        logger.LogWarning("Processing role deletion notification (Soft Delete). Role ID: {RoleId}, Name: {RoleName}",
            @event.Id, @event.Name);

        try
        {
            // 2. İş Mantığı: Bildirim mesajını hazırla
            var message = $"Kritik Bilgilendirme: '{@event.Name}' isimli rol sistemde pasif duruma getirilmiştir (Soft Deleted). ID: {@event.Id}";

            // 3. Eylem: PlatformService üzerinden yöneticiye e-posta gönder
            await platformService.SendEmailAsync("admin@iztek.com", message);

            // 4. Başarı Logu
            logger.LogInformation("Role deletion notification sent successfully for Role ID: {RoleId}", @event.Id);
        }
        catch (Exception ex)
        {
            // 5. Hata Logu
            logger.LogError(ex, "An error occurred while sending role deletion notification. Role ID: {RoleId}", @event.Id);
        }
    }
}