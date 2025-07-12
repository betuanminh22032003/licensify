using LicenseService.Domain.Common;

namespace LicenseService.Domain.ValueObjects;

public class CustomerId : ValueObject
{
    public Guid Value { get; private set; }

    private CustomerId(Guid value)
    {
        Value = value;
    }

    public static CustomerId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(value));

        return new CustomerId(value);
    }

    public static CustomerId NewId() => new(Guid.NewGuid());

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(CustomerId customerId) => customerId.Value;
    public static implicit operator CustomerId(Guid value) => Create(value);
}
