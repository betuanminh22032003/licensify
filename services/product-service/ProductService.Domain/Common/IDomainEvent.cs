namespace ProductService.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
