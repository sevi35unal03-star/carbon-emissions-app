namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(128);

        builder.Property(p => p.Category)
               .HasConversion(c => c.ToString(),
                              p => Enum.Parse<CategoryType>(p))
               .HasMaxLength(32);
    }
}