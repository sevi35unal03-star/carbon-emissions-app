

namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class UserPollResultConfiguration : IEntityTypeConfiguration<UserPollResult>
{
    public void Configure(EntityTypeBuilder<UserPollResult> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Surname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.PollSetId)
            .IsRequired();

        builder.Property(x => x.TotalScore)
            .IsRequired();

        builder.Property(x => x.TreeCount)
            .IsRequired();

        builder.Property(x => x.Month)
            .IsRequired();

        builder.Property(x => x.Year)
            .IsRequired();

        // Kullanıcı ayda bir kez aynı anketi cevaplıyor — unique constraint
        builder.HasIndex(x => new { x.UserId, x.PollSetId, x.Month, x.Year })
            .IsUnique();

        // UserPollAnswer'lar cascade delete ile silinir
        builder.HasMany(x => x.Answers)
            .WithOne(x => x.UserPollResult)
            .HasForeignKey(x => x.UserPollResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("UserPollResults");
    }
}