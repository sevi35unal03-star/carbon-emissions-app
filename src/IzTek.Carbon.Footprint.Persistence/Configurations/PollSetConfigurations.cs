namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class PollSetConfigurations : IEntityTypeConfiguration<PollSet>
{
    public void Configure(EntityTypeBuilder<PollSet> builder)
    {
        builder.ToTable("PollSets");

        builder.Property(x => x.Name)
               .HasMaxLength(128)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(512);

        builder.Property(x => x.DisplayOrder)
               .IsRequired();

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);
        // ← HasMany kaldırıldı
    }
}