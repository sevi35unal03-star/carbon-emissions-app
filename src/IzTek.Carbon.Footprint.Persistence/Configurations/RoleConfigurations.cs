namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Name alanı için Product'taki gibi uzunluk belirleyelim
        builder.Property(x => x.Name)
               .HasMaxLength(128)
               .IsRequired();

        // Product.cs'deki Enum-String dönüşüm mantığının aynısı
        builder.Property(p => p.Type)
               .HasConversion(t => t.ToString(),
                             t => Enum.Parse<RoleType>(t))
               .HasMaxLength(32)
               .IsRequired();

        // Rol isimlerinin benzersiz olması kritik bir kuraldır
        builder.HasIndex(x => x.Name)
               .IsUnique();
    }
}