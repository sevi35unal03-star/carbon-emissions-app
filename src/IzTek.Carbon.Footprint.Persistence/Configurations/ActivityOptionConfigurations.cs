namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class ActivityOptionConfiguration : IEntityTypeConfiguration<ActivityOption>
{
    public void Configure(EntityTypeBuilder<ActivityOption> builder)
    {
        builder.HasKey(x => x.Id);

        // Ana ilişki: ActivityOption → ActivityQuestion (hangi soruya ait)
        builder.HasOne(x => x.ActivityQuestion)
               .WithMany(x => x.Options)
               .HasForeignKey(x => x.ActivityQuestionId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(true);

        // Kırılım ilişkisi: ActivityOption → NextQuestion (sonraki soru)
        builder.HasOne(x => x.NextQuestion)
               .WithMany()                          // ← Options collection'ıyla karıştırmaması için boş bırakıyoruz
               .HasForeignKey(x => x.NextQuestionId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        builder.ToTable("ActivityOptions");
    }
}