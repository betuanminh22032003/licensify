using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        // Configure primary key
        builder.HasKey(p => p.Id);

        // Configure ProductId
        builder.Property(p => p.Id)
            .IsRequired()
            .HasMaxLength(36);

        // Configure ProductName
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Configure ProductVersion
        builder.Property(p => p.Version)
            .IsRequired()
            .HasMaxLength(50);

        // Configure Description
        builder.Property(p => p.Description)
            .HasMaxLength(500);

        // Configure IsActive
        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure audit fields
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .IsRequired();

        builder.Property(p => p.UpdatedBy);

        // Configure indexes
        builder.HasIndex(p => new { p.Name, p.Version })
            .IsUnique()
            .HasDatabaseName("IX_Products_Name_Version");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Products_IsActive");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_Products_CreatedAt");

        // Ignore domain events (they are not persisted)
        builder.Ignore(p => p.DomainEvents);
    }
}
