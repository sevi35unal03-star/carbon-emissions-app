namespace IzTek.Carbon.Footprint.Infrastructure.Persistence.Configurations;

public class TreeDefinitionConfiguration : IEntityTypeConfiguration<TreeDefinition>
{
    public void Configure(EntityTypeBuilder<TreeDefinition> builder)
    {
        builder.ToTable("TreeDefinitions");

        builder.HasKey(x => x.PollQuestionId);

        builder.Property(x => x.PointUnit)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasComment("Kaç puana karşılık geldiğini belirtir. Örn: 10");

        builder.Property(x => x.TreeCount)
            .IsRequired()
            .HasComment("PointUnit puana karşılık gelen ağaç sayısı. Örn: 2");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Sistemde yalnızca bir aktif tanım olabilir");

        // Audit fields - BaseAuditableEntity'den geliyor
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.LastModifiedAt);

        builder.Property(x => x.LastModifiedBy)
            .HasMaxLength(100);

        // Aynı anda sadece 1 aktif tanım olabilmesi için index
        builder.HasIndex(x => x.IsActive)
            .HasFilter("\"IsActive\" = true")
            .IsUnique()
            .HasDatabaseName("IX_TreeDefinitions_SingleActive");
    }
}