namespace Licensify.Shared.Events;

public abstract record BaseEvent(
    Guid Id,
    DateTime Timestamp,
    string EventType
);

public record UserLoggedInEvent(
    Guid Id,
    DateTime Timestamp,
    Guid UserId,
    string Email,
    string IpAddress
) : BaseEvent(Id, Timestamp, "UserLoggedIn");

public record LicenseCreatedEvent(
    Guid Id,
    DateTime Timestamp,
    Guid LicenseId,
    Guid ProductId,
    string LicenseKey,
    Guid CreatedBy,
    DateTime ExpiresAt,
    int MaxDevices
) : BaseEvent(Id, Timestamp, "LicenseCreated");

public record LicenseValidatedEvent(
    Guid Id,
    DateTime Timestamp,
    Guid LicenseId,
    string LicenseKey,
    bool IsValid,
    string? DeviceId,
    string? IpAddress,
    string? ErrorMessage
) : BaseEvent(Id, Timestamp, "LicenseValidated");

public record LicenseRevokedEvent(
    Guid Id,
    DateTime Timestamp,
    Guid LicenseId,
    string LicenseKey,
    Guid RevokedBy,
    string Reason
) : BaseEvent(Id, Timestamp, "LicenseRevoked");

public record ProductCreatedEvent(
    Guid Id,
    DateTime Timestamp,
    Guid ProductId,
    string Name,
    string Version,
    Guid CreatedBy
) : BaseEvent(Id, Timestamp, "ProductCreated");

public record ProductUpdatedEvent(
    Guid Id,
    DateTime Timestamp,
    Guid ProductId,
    string Name,
    string Version,
    Guid UpdatedBy,
    object? OldValues,
    object? NewValues
) : BaseEvent(Id, Timestamp, "ProductUpdated");
