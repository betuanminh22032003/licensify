using Microsoft.EntityFrameworkCore;
using LicenseService.Domain.Entities;
using LicenseService.Infrastructure.Persistence.Configurations;

namespace LicenseService.Infrastructure.Persistence;

public class LicenseDbContext : DbContext
{
    public LicenseDbContext(DbContextOptions<LicenseDbContext> options) : base(options)
    {
    }

    public DbSet<License> Licenses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new LicenseConfiguration());

        // Set schema
        modelBuilder.HasDefaultSchema("licenses");
    }
}
