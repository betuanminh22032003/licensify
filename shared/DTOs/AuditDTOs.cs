namespace Licensify.Shared.DTOs;

public record AuditLogDto(
    Guid Id,
    string EventType,
    string UserId,
    string? UserEmail,
    string EntityType,
    string EntityId,
    string Action,
    object? OldValues,
    object? NewValues,
    string? IpAddress,
    string? UserAgent,
    DateTime Timestamp
);

public record AuditLogSearchRequest(
    string? EventType = null,
    string? UserId = null,
    string? EntityType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 50
);
