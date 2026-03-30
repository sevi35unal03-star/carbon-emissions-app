namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class PollOptionConfigurations : IEntityTypeConfiguration<PollOption>
{
    public void Configure(EntityTypeBuilder<PollOption> builder)
    {
        builder.ToTable("PollOptions");

        builder.Property(x => x.Text)
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(x => x.CarbonValue)
               .IsRequired();

        // Ana ilişki
        builder.HasOne(x => x.PollQuestion)
               .WithMany(x => x.Options)
               .HasForeignKey(x => x.PollQuestionId)
               .OnDelete(DeleteBehavior.Cascade);

        // NextQuestion ilişkisi
        builder.HasOne<PollQuestion>()
               .WithMany()
               .HasForeignKey(x => x.NextPollQuestionId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
    }
}