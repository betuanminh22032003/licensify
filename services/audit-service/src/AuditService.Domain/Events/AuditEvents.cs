using AuditService.Domain.Common;
using AuditService.Domain.ValueObjects;
using AuditService.Domain.Enums;

namespace AuditService.Domain.Events;

public record AuditLogCreatedEvent(
    AuditId AuditId, 
    AuditAction Action, 
    EntityType EntityType) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
