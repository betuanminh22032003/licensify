using ProductService.Domain.Common;

namespace ProductService.Domain.ValueObjects;

public class ProductId : ValueObject
{
    public Guid Value { get; private set; }

    private ProductId(Guid value)
    {
        Value = value;
    }

    public static ProductId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(value));

        return new ProductId(value);
    }

    public static ProductId NewId()
    {
        return new ProductId(Guid.NewGuid());
    }

    public static implicit operator Guid(ProductId productId) => productId.Value;
    public static implicit operator ProductId(Guid value) => Create(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
