using Iztek.Carbon.Footprint.Domain.Entities;

public class ActivityQuestionConfiguration : IEntityTypeConfiguration<ActivityQuestion>
{
    public void Configure(EntityTypeBuilder<ActivityQuestion> builder)
    {
        builder.HasKey(x => x.PollQuestionId);

        // Bir sorunun birden fazla seçeneği vardır
        builder.HasMany(x => x.Options)
               .WithOne(x => x.ActivityQuestion)
               .HasForeignKey(x => x.ActivityQuestionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

