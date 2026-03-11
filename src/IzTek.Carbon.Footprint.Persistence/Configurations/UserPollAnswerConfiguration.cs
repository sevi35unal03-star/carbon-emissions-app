namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class UserPollAnswerConfiguration : IEntityTypeConfiguration<UserPollAnswer>
{
    public void Configure(EntityTypeBuilder<UserPollAnswer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuestionText)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.SelectedOptionText)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CarbonValue)
            .IsRequired();

        builder.HasOne(x => x.UserPollResult)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.UserPollResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("UserPollAnswers");
    }
}