using AuditService.Domain.Entities;
using AuditService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Infrastructure.Data;

public class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
    }
}
