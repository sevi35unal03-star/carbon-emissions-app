<<<<<<< HEAD
﻿

namespace IzTek.Carbon.Footprint.Persistence.Configurations; 
=======
﻿namespace IzTek.Carbon.Footprint.Persistence.Configurations; // ✅ namespace eklendi
>>>>>>> 44df4230f9ac6b55182e1f58c5c75b07d4c62e99

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