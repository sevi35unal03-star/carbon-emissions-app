

namespace IzTek.Carbon.Footprint.Persistence.Configurations; // ✅ namespace eklendi

public class ActivityQuestionConfiguration : IEntityTypeConfiguration<ActivityQuestion>
{
    public void Configure(EntityTypeBuilder<ActivityQuestion> builder)
    {
        builder.HasKey(x => x.Id); // ✅ PollQuestionId → Id

        // Bir sorunun birden fazla seçeneği vardır
        builder.HasMany(x => x.Options)
               .WithOne(x => x.ActivityQuestion)
               .HasForeignKey(x => x.ActivityQuestionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("ActivityQuestions");
    }
}