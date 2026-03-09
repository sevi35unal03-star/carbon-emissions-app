namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class PollQuestionConfigurations : IEntityTypeConfiguration<PollQuestion>
{
    public void Configure(EntityTypeBuilder<PollQuestion> builder)
    {
        builder.ToTable("PollQuestions");

        builder.Property(x => x.Text)
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(x => x.DisplayOrder)
               .IsRequired();

        builder.HasMany(x => x.Options)
               .WithOne()
               .HasForeignKey(x => x.PollQuestionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}