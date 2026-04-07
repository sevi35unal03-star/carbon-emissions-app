using IzTek.Carbon.Footprint.Domain.Events.Activity;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Notifications;
/// <summary>
/// Kullanıcı günlük aktivite akışını tamamladığında tetiklenir.
/// Tüm sorular cevaplandıktan sonra kullanıcıya platform üzerinden
/// günlük karbon skoru bildirimi gönderir.
/// </summary>
public class ActivityAnsweredDomainEventHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IPlatformService _platformService;

    public ActivityAnsweredDomainEventHandler(
        IApplicationDbContext context,
        IPlatformService platformService)
    {
        _context = context;
        _platformService = platformService;
    }

    public async Task Handle(ActivityAnsweredDomainEvent @event, CancellationToken ct)
    {
        if (!@event.IsFlowCompleted) return;

        var today = DateTime.UtcNow.Date;
        var totalCarbon = await _context.UserActivityLogs
            .Where(x => x.UserId == @event.UserId &&
                        x.ActivityDate >= today &&
                        x.ActivityDate < today.AddDays(1))
            .SumAsync(x => x.TotalCarbonScore, ct);

        await _platformService.SendPushAsync(
            userId: @event.UserId,
            title: "Günlük aktiviteler tamamlandı!",
            body: $"Bugünkü karbon skorun: {totalCarbon}",
            ct: ct);
    }
}