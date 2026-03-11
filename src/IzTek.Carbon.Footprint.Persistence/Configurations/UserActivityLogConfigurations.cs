
namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class UserActivityLogConfigurations : IEntityTypeConfiguration<UserActivityLog>
{
    public void Configure(EntityTypeBuilder<UserActivityLog> builder)
    {
        builder.Property(x => x.SelectedOptionText)
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(x => x.CarbonValue)
               .IsRequired();

        // ek kısıtlamalar veya indexler eklenebilir.
        builder.Property(x => x.UserId)
               .IsRequired();

        builder.Property(x => x.ActivityQuestionId)
               .IsRequired();

        // Eğer projenizde Log seviyeleri veya tipleri Enum ise örnekteki gibi kullanabilirsiniz:
        // builder.Property(p => p.Status)
        //        .HasConversion(s => s.ToString(),
        //                       p => Enum.Parse<LogStatusType>(p))
        //        .HasMaxLength(32);

        // İlişki Tanımlamaları (Opsiyonel ama veri bütünlüğü için önerilir)
        builder.HasOne<ActivityQuestion>()
               .WithMany()
               .HasForeignKey(x => x.ActivityQuestionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}