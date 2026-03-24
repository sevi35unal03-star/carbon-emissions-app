namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class AppAssetConfiguration : IEntityTypeConfiguration<AppAsset>
{
    public void Configure(EntityTypeBuilder<AppAsset> builder)
    {
        builder.ToTable("AppAssets");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AssetType)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("HomeHero, HomeTreeIcon, CarbonCalculate, AppLogo, DonationSuccess");

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("MinIO'daki dosya adı");

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // Her AssetType'tan yalnızca bir aktif kayıt olabilir
        builder.HasIndex(x => x.AssetType)
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique()
            .HasDatabaseName("IX_AppAssets_AssetType_Unique");
    }
}