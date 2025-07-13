using ProductService.Domain.Common;

namespace ProductService.Domain.ValueObjects;

public class ProductName : ValueObject
{
    public string Value { get; private set; }

    private ProductName(string value)
    {
        Value = value;
    }

    public static ProductName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Product name cannot exceed 100 characters", nameof(name));

        if (name.Length < 3)
            throw new ArgumentException("Product name must be at least 3 characters", nameof(name));

        return new ProductName(name.Trim());
    }

    public static implicit operator string(ProductName productName) => productName.Value;
    public static implicit operator ProductName(string value) => Create(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
