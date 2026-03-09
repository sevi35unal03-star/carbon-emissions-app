namespace IzTek.Carbon.Footprint.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string? Name { get; private set; }
    public CategoryType Category { get; private set; }

    protected Product()
    {
    }

    public Product(string name, CategoryType category)
    {
        Name = name;
        Category = category;
    }
}