

namespace IzTek.Carbon.Footprint.Persistence.Configurations; // ✅ namespace eklendi

public class ActivityOptionConfiguration : IEntityTypeConfiguration<ActivityOption>
{
    public void Configure(EntityTypeBuilder<ActivityOption> builder)
    {
        builder.HasKey(x => x.Id); // ✅ PollQuestionId → Id

        // KIRILIM NOKTASI: Opsiyon seçilince açılacak olan soru
        builder.HasOne(x => x.NextQuestion)
               .WithOne()
               .HasForeignKey<ActivityOption>(x => x.NextQuestionId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.ToTable("ActivityOptions");
    }
}