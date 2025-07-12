using LicenseService.Domain.Common;
using LicenseService.Domain.ValueObjects;
using LicenseService.Domain.Enums;
using LicenseService.Domain.Events;

namespace LicenseService.Domain.Entities;

public class License : BaseEntity
{
    private License() { } // EF Constructor

    public LicenseId Id { get; private set; } = null!;
    public ProductId ProductId { get; private set; } = null!;
    public CustomerId CustomerId { get; private set; } = null!;
    public LicenseKey LicenseKey { get; private set; } = null!;
    public LicenseType Type { get; private set; }
    public LicenseStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public DateTime? LastValidatedAt { get; private set; }
    public int MaxUsers { get; private set; }
    public int CurrentUsers { get; private set; }
    public string? Notes { get; private set; }

    public static License Create(
        ProductId productId,
        CustomerId customerId,
        LicenseType type,
        DateTime expiresAt,
        int maxUsers = 1,
        string? notes = null)
    {
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));

        if (maxUsers <= 0)
            throw new ArgumentException("Max users must be greater than zero", nameof(maxUsers));

        var license = new License
        {
            Id = LicenseId.NewId(),
            ProductId = productId,
            CustomerId = customerId,
            LicenseKey = LicenseKey.Generate(),
            Type = type,
            Status = LicenseStatus.Pending,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            MaxUsers = maxUsers,
            CurrentUsers = 0,
            Notes = notes
        };

        license.AddDomainEvent(new LicenseCreatedEvent(license.Id, license.ProductId, license.CustomerId));
        return license;
    }

    public void Activate()
    {
        if (Status == LicenseStatus.Active)
            throw new InvalidOperationException("License is already active");

        if (Status == LicenseStatus.Revoked)
            throw new InvalidOperationException("Cannot activate a revoked license");

        if (IsExpired())
            throw new InvalidOperationException("Cannot activate an expired license");

        Status = LicenseStatus.Active;
        ActivatedAt = DateTime.UtcNow;
        UpdateTimestamp();

        AddDomainEvent(new LicenseActivatedEvent(Id, CustomerId));
    }

    public void Suspend(string reason)
    {
        if (Status != LicenseStatus.Active)
            throw new InvalidOperationException("Can only suspend active licenses");

        Status = LicenseStatus.Suspended;
        Notes = reason;
        UpdateTimestamp();

        AddDomainEvent(new LicenseSuspendedEvent(Id, CustomerId, reason));
    }

    public void Revoke(string reason)
    {
        if (Status == LicenseStatus.Revoked)
            throw new InvalidOperationException("License is already revoked");

        Status = LicenseStatus.Revoked;
        Notes = reason;
        UpdateTimestamp();

        AddDomainEvent(new LicenseRevokedEvent(Id, CustomerId, reason));
    }

    public bool Validate()
    {
        LastValidatedAt = DateTime.UtcNow;
        UpdateTimestamp();

        if (IsExpired())
        {
            Status = LicenseStatus.Expired;
            AddDomainEvent(new LicenseExpiredEvent(Id, CustomerId));
            return false;
        }

        return Status == LicenseStatus.Active;
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    public bool CanAddUser() => CurrentUsers < MaxUsers && Status == LicenseStatus.Active;

    public void AddUser()
    {
        if (!CanAddUser())
            throw new InvalidOperationException("Cannot add more users to this license");

        CurrentUsers++;
        UpdateTimestamp();
    }

    public void RemoveUser()
    {
        if (CurrentUsers <= 0)
            throw new InvalidOperationException("No users to remove");

        CurrentUsers--;
        UpdateTimestamp();
    }

    public void ExtendExpiration(DateTime newExpirationDate)
    {
        if (newExpirationDate <= ExpiresAt)
            throw new ArgumentException("New expiration date must be after current expiration date");

        var oldExpiration = ExpiresAt;
        ExpiresAt = newExpirationDate;
        
        if (Status == LicenseStatus.Expired)
        {
            Status = LicenseStatus.Active;
        }

        UpdateTimestamp();
        AddDomainEvent(new LicenseExtendedEvent(Id, CustomerId, oldExpiration, newExpirationDate));
    }
}
