using MediatR;
using AuditService.Domain.ValueObjects;
using AuditService.Domain.Enums;

namespace AuditService.Application.Commands.Audit;

public record CreateAuditLogCommand(
    Guid? UserId,
    string? UserName,
    EntityType EntityType,
    string? EntityId,
    AuditAction Action,
    string Description,
    AuditSeverity Severity = AuditSeverity.Information,
    string? OldValues = null,
    string? NewValues = null,
    string? IpAddress = null,
    string? UserAgent = null,
    string? Source = null,
    string? AdditionalData = null
) : IRequest<CreateAuditLogResponse>;

public record CreateAuditLogResponse(
    Guid AuditId,
    DateTime Timestamp
);
