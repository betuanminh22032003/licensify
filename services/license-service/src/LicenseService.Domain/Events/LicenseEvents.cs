using LicenseService.Domain.Common;
using LicenseService.Domain.ValueObjects;

namespace LicenseService.Domain.Events;

public record LicenseCreatedEvent(
    LicenseId LicenseId, 
    ProductId ProductId, 
    CustomerId CustomerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LicenseActivatedEvent(
    LicenseId LicenseId, 
    CustomerId CustomerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LicenseSuspendedEvent(
    LicenseId LicenseId, 
    CustomerId CustomerId, 
    string Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LicenseRevokedEvent(
    LicenseId LicenseId, 
    CustomerId CustomerId, 
    string Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LicenseExpiredEvent(
    LicenseId LicenseId, 
    CustomerId CustomerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LicenseExtendedEvent(
    LicenseId LicenseId, 
    CustomerId CustomerId, 
    DateTime OldExpiration, 
    DateTime NewExpiration) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record LicenseValidatedEvent(
    LicenseId LicenseId, 
    CustomerId CustomerId, 
    bool IsValid) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
