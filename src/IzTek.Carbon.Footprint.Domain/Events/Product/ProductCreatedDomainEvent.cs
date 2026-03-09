namespace IzTek.Carbon.Footprint.Domain.Events.Product;

public class ProductCreatedDomainEvent : BaseEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}