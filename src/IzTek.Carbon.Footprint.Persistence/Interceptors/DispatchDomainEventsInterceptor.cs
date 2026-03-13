namespace IzTek.Carbon.Footprint.Persistence.Interceptors;

public class DispatchDomainEventsInterceptor(IMessageBus bus) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEventsAsync(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(DbContext? context)
    {
        if (context == null) return;

        // Artık IDomainEventContainer tarıyor — BaseEntity VE User ikisi de yakalanır
        var containers = context.ChangeTracker
            .Entries<IDomainEventContainer>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = containers
            .SelectMany(e => e.DomainEvents)
            .ToList();

        containers.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await bus.PublishAsync(domainEvent);
        }
    }
}