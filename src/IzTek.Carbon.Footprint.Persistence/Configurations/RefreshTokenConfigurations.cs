namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.Property(x => x.Token)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(x => x.RevokedReason)
               .HasMaxLength(256);

        builder.HasIndex(x => x.Token)
               .IsUnique();

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(x => x.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}