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


        builder.HasOne<PollQuestion>()
               .WithMany()
               .HasForeignKey(x => x.NextPollQuestionId)
               .OnDelete(DeleteBehavior.Restrict); // Hedef soru silinirse bu seçenek boşa düşmesin diye kısıtlıyoruz.
    }
}