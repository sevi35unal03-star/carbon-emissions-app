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

        // ← ekle
        builder.HasOne(x => x.PollSet)
               .WithMany(x => x.Questions)
               .HasForeignKey(x => x.PollSetId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}