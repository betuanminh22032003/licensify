using AuditService.Domain.Entities;
using AuditService.Domain.ValueObjects;
using AuditService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditService.Infrastructure.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion<string>();

        builder.Property(x => x.EntityId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Severity)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.UserId)
            .HasConversion(
                v => v != null ? v.Value : (Guid?)null,
                v => v != null ? UserId.Create(v.Value) : null);

        builder.Property(x => x.UserName)
            .HasMaxLength(100);

        builder.Property(x => x.OldValues)
            .HasColumnType("jsonb");

        builder.Property(x => x.NewValues)
            .HasColumnType("jsonb");

        builder.Property(x => x.Timestamp)
            .IsRequired();

        // Indexes for better query performance
        builder.HasIndex(x => x.EntityType);
        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => new { x.EntityType, x.EntityId });
    }
}
