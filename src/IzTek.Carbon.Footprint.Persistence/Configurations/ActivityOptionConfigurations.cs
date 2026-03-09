using Iztek.Carbon.Footprint.Domain.Entities;

public class ActivityOptionConfiguration : IEntityTypeConfiguration<ActivityOption>
{
    public void Configure(EntityTypeBuilder<ActivityOption> builder)
    {
        builder.HasKey(x => x.PollQuestionId);

        // KIRILIM NOKTASI: Opsiyon seçilince açılacak olan soru
        builder.HasOne(x => x.NextQuestion)
               .WithOne() // Her soru sadece bir opsiyonun "devamı" olabilir (opsiyonel)
               .HasForeignKey<ActivityOption>(x => x.NextQuestionId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}