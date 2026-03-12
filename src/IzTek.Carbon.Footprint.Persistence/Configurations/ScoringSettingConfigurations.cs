namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class ScoringSettingConfigurations : IEntityTypeConfiguration<ScoringSetting>
{
    public void Configure(EntityTypeBuilder<ScoringSetting> builder)
    {
        builder.ToTable("ScoringSettings");

        builder.Property(x => x.Key)
               .HasMaxLength(64)
               .IsRequired();

        builder.Property(x => x.Value)
               .IsRequired();

        // Senin örneğindeki gibi Enum -> String dönüşümü
        builder.Property(p => p.Category)
               .HasConversion(c => c.ToString(),
                              p => Enum.Parse<ScoringCategory>(p))
               .HasMaxLength(32);

        // Genellikle bu ayarlar sistem başlangıcında hazır olmalıdır (Seeding yapılabilir)
    }
}