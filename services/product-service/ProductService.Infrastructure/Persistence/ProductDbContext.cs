using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Common;
using ProductService.Infrastructure.Persistence.Configurations;

namespace ProductService.Infrastructure.Persistence;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new ProductConfiguration());

        // Configure value objects and entities
        ConfigureValueObjects(modelBuilder);
    }

    private static void ConfigureValueObjects(ModelBuilder modelBuilder)
    {
        // Configure ProductId value object
        modelBuilder.Entity<Product>()
            .Property(e => e.Id)
            .HasConversion(
                v => v.Value,
                v => ProductService.Domain.ValueObjects.ProductId.Create(v));

        // Configure ProductName value object
        modelBuilder.Entity<Product>()
            .Property(e => e.Name)
            .HasConversion(
                v => v.Value,
                v => ProductService.Domain.ValueObjects.ProductName.Create(v));

        // Configure ProductVersion value object
        modelBuilder.Entity<Product>()
            .Property(e => e.Version)
            .HasConversion(
                v => v.Value,
                v => ProductService.Domain.ValueObjects.ProductVersion.Create(v));
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Handle domain events before saving
        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .Select(x => x.Entity)
            .SelectMany(x => x.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Clear domain events after saving
        foreach (var entity in ChangeTracker.Entries<BaseEntity>().Select(x => x.Entity))
        {
            entity.ClearDomainEvents();
        }

        // TODO: Publish domain events via MediatR
        // foreach (var domainEvent in domainEvents)
        // {
        //     await _mediator.Publish(domainEvent, cancellationToken);
        // }

        return result;
    }
}
