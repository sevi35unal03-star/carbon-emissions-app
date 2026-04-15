namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class ActivityOptionConfiguration : IEntityTypeConfiguration<ActivityOption>
{
    public void Configure(EntityTypeBuilder<ActivityOption> builder)
    {
        builder.HasKey(x => x.Id);

        // ✅ WithMany — birden fazla option aynı soruya işaret edebilir
        builder.HasOne(x => x.NextQuestion)
               .WithMany()
               .HasForeignKey(x => x.NextQuestionId)
               .OnDelete(DeleteBehavior.Restrict) // SetNull yerine Restrict
               .IsRequired(false);

        builder.ToTable("ActivityOptions");
    }
}