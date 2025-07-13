using ProductService.Domain.Common;
using ProductService.Domain.ValueObjects;
using ProductService.Domain.Events;

namespace ProductService.Domain.Entities;

public class Product : BaseEntity
{
    private Product() { } // Private constructor for EF Core

    public ProductId Id { get; private set; } = null!;
    public ProductName Name { get; private set; } = null!;
    public ProductVersion Version { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // Factory method to create new product
    public static Product Create(ProductName name, ProductVersion version, string? description = null, Guid? createdBy = null)
    {
        var product = new Product
        {
            Id = ProductId.NewId(),
            Name = name,
            Version = version,
            Description = description,
            IsActive = true,
            CreatedBy = createdBy ?? Guid.Empty
        };

        // Raise domain event
        product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.Version));
        
        return product;
    }

    // Business methods
    public void UpdateDetails(ProductName name, ProductVersion version, string? description, Guid updatedBy)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot update inactive product");

        Name = name;
        Version = version;
        Description = description;
        UpdatedBy = updatedBy;
        UpdateTimestamp();

        // Raise domain event
        AddDomainEvent(new ProductUpdatedEvent(Id, Name, Version));
    }

    public void Activate(Guid updatedBy)
    {
        if (IsActive)
            throw new InvalidOperationException("Product is already active");

        IsActive = true;
        UpdatedBy = updatedBy;
        UpdateTimestamp();

        AddDomainEvent(new ProductActivatedEvent(Id, Name));
    }

    public void Deactivate(Guid updatedBy)
    {
        if (!IsActive)
            throw new InvalidOperationException("Product is already inactive");

        IsActive = false;
        UpdatedBy = updatedBy;
        UpdateTimestamp();

        AddDomainEvent(new ProductDeactivatedEvent(Id, Name));
    }

    public void ChangeVersion(ProductVersion newVersion, Guid updatedBy)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot change version of inactive product");

        var oldVersion = Version;
        Version = newVersion;
        UpdatedBy = updatedBy;
        UpdateTimestamp();

        AddDomainEvent(new ProductVersionChangedEvent(Id, Name, oldVersion, newVersion));
    }
}
