using MediatR;

namespace AuditService.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
