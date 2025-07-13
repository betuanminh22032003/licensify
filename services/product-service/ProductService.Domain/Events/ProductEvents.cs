using ProductService.Domain.Common;
using ProductService.Domain.ValueObjects;

namespace ProductService.Domain.Events;

public record ProductCreatedEvent(
    ProductId ProductId,
    ProductName Name,
    ProductVersion Version
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductUpdatedEvent(
    ProductId ProductId,
    ProductName Name,
    ProductVersion Version
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductActivatedEvent(
    ProductId ProductId,
    ProductName Name
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductDeactivatedEvent(
    ProductId ProductId,
    ProductName Name
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductVersionChangedEvent(
    ProductId ProductId,
    ProductName Name,
    ProductVersion OldVersion,
    ProductVersion NewVersion
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
