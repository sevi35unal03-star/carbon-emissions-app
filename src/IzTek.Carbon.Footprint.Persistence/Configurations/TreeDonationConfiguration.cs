

namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class TreeDonationConfiguration : IEntityTypeConfiguration<TreeDonation>
{
    public void Configure(EntityTypeBuilder<TreeDonation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.TreeCount)
            .IsRequired();

        builder.Property(x => x.PointsSpent)
            .IsRequired();

        builder.Property(x => x.DonationDate)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.ToTable("TreeDonations");
    }
}