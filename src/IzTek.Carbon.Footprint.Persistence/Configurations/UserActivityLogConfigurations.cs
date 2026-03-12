namespace IzTek.Carbon.Footprint.Persistence.Configurations;

/// <summary>
/// UserActivityLog — Admin raporlama ve audit amaçlı.
/// Navigation property'ler içerir (User, ActivityQuestion, ActivityOption).
///
/// UserActivityAnswer — Domain event + takvim/pending sorguları için.
/// Navigation property içermez, snapshot CarbonValue tutar.
///
/// İkisi birbirini tamamlar, çakışmaz:
/// - UserActivityAnswer: hızlı sorgu, event fırlatma, takvim
/// - UserActivityLog: admin panel, detaylı raporlama, join'li görünümler
/// </summary>
public class UserActivityLogConfiguration : IEntityTypeConfiguration<UserActivityLog>
{
    public void Configure(EntityTypeBuilder<UserActivityLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TotalCarbonScore)
            .IsRequired();

        builder.Property(x => x.CarbonValue)
            .IsRequired();

        builder.Property(x => x.SelectedOptionText)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ActivityDate)
            .IsRequired();

        // Admin raporlarında UserId + ActivityDate üzerinden sık sorgu yapılır
        builder.HasIndex(x => new { x.UserId, x.ActivityDate });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ActivityQuestion)
            .WithMany()
            .HasForeignKey(x => x.ActivityQuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ActivityOption)
            .WithMany()
            .HasForeignKey(x => x.ActivityOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("UserActivityLogs");
    }
}