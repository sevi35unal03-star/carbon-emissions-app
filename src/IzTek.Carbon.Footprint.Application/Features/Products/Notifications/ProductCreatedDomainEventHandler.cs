namespace IzTek.Carbon.Footprint.Application.Features.Products.Notifications;

public static class ProductCreatedDomainEventHandler
{
    public static async Task HandleAsync(
        ProductCreatedDomainEvent @event,
        IPlatformService platformService,
        ILogger<ProductCreatedDomainEvent> logger, // Loglama servisi enjekte edildi
        CancellationToken cancellationToken)
    {
        // 1. İşlem Başlangıcı Logu
        logger.LogInformation("Yeni ürün oluşturma bildirimi işleniyor. Ürün ID: {ProductId}, İsim: {ProductName}", @event.Id, @event.Name);

        try
        {
            // 2. İş Mantığı: Bildirim içeriğini hazırla
            var message = $"Yeni bir ürün sisteme eklendi: {@event.Name} (ID: {@event.Id})";

            // 3. Aksiyon: Mail gönder
            await platformService.SendEmailAsync("admin@iztek.com", message);

            // 4. Başarı Logu
            logger.LogInformation("Ürün oluşturma maili başarıyla gönderildi: {ProductId}", @event.Id);
        }
        catch (Exception ex)
        {
            // 5. Hata Logu: Mail gönderimi başarısız olsa bile ana işlem etkilenmez ama hata kaydedilir
            logger.LogError(ex, "Ürün oluşturma bildirimi gönderilirken hata oluştu. Ürün ID: {ProductId}", @event.Id);
        }
    }
}