using IzTek.Carbon.Footprint.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IzTek.Carbon.Footprint.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Name).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Surname).HasMaxLength(50).IsRequired();

        // TCKN benzersiz olmalı ve 11 hane sınırlaması veritabanında da olmalı
        builder.Property(u => u.IdentityNumber).HasMaxLength(11).IsFixedLength().IsRequired();
        builder.HasIndex(u => u.IdentityNumber).IsUnique();

        builder.Property(u => u.PhoneNumber).HasMaxLength(15);

        // Global Query Filter: Tüm sorgularda IsDeleted = false olanları getirir
        // Admin sorgusunda .IgnoreQueryFilters() diyerek silinmişleri görebilirsin.
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}