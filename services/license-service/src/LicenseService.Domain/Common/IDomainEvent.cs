using MediatR;

namespace LicenseService.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
