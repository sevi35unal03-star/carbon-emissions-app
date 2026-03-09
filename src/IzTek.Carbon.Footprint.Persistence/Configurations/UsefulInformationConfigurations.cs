namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class UsefulInformationConfigurations : IEntityTypeConfiguration<UsefulInformation>
{
    public void Configure(EntityTypeBuilder<UsefulInformation> builder)
    {
        // Başlık alanı [FG1]: Uygulama ana sayfasında soru başlığı olarak gözükecek
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(256);

        // Bilgi Detay [FG1]: Uzun metin girişi yapılabileceği için max uzunluk yerine nvarchar(max)
        builder.Property(x => x.Content)
            .IsRequired();

        // Aktif? [FG1]: Varsayılan olarak pasif veya aktif başlayabilir
        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        // Audit ve Tarihçe için tablo ismini sabitleyebilirsin
        builder.ToTable("UsefulInformations");
    }
}