using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LicenseService.Domain.Entities;
using LicenseService.Domain.ValueObjects;
using LicenseService.Domain.Enums;

namespace LicenseService.Infrastructure.Persistence.Configurations;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ToTable("licenses");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Configure Value Objects
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => LicenseId.Create(value))
            .IsRequired();

        builder.Property(x => x.ProductId)
            .HasConversion(
                id => id.Value,
                value => ProductId.Create(value))
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .HasConversion(
                id => id.Value,
                value => CustomerId.Create(value))
            .IsRequired();

        builder.Property(x => x.LicenseKey)
            .HasConversion(
                key => key.Value,
                value => LicenseKey.Create(value))
            .HasMaxLength(100)
            .IsRequired();

        // Configure Enums
        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        // Configure Properties
        builder.Property(x => x.IssuedAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.ActivatedAt);

        builder.Property(x => x.LastValidatedAt);

        builder.Property(x => x.MaxUsers)
            .IsRequired();

        builder.Property(x => x.CurrentUsers)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.LicenseKey)
            .IsUnique()
            .HasDatabaseName("IX_licenses_license_key");

        builder.HasIndex(x => x.CustomerId)
            .HasDatabaseName("IX_licenses_customer_id");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_licenses_product_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_licenses_status");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_licenses_expires_at");

        // Ignore Domain Events
        builder.Ignore(x => x.DomainEvents);
    }
}
