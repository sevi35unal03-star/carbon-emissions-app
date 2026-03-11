

namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class UserActivityAnswerConfiguration : IEntityTypeConfiguration<UserActivityAnswer>
{
    public void Configure(EntityTypeBuilder<UserActivityAnswer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.QuestionId)
            .IsRequired();

        builder.Property(x => x.SelectedOptionId)
            .IsRequired();

        builder.Property(x => x.AnsweredAt)
            .IsRequired();

        // Takvim ve pending sorgularında UserId + AnsweredAt üzerinden sık sorgu yapılır
        builder.HasIndex(x => new { x.UserId, x.AnsweredAt });

        builder.ToTable("UserActivityAnswers");
    }
}