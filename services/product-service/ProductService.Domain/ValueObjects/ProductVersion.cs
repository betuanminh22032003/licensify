using ProductService.Domain.Common;

namespace ProductService.Domain.ValueObjects;

public class ProductVersion : ValueObject
{
    public string Value { get; private set; }

    private ProductVersion(string value)
    {
        Value = value;
    }

    public static ProductVersion Create(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Product version cannot be empty", nameof(version));

        if (version.Length > 50)
            throw new ArgumentException("Product version cannot be longer than 50 characters", nameof(version));

        // Basic version validation (you can enhance this with regex)
        if (!IsValidVersion(version))
            throw new ArgumentException("Invalid version format. Expected format: x.x.x or x.x.x.x", nameof(version));

        return new ProductVersion(version.Trim());
    }

    private static bool IsValidVersion(string version)
    {
        // Simple version validation - can be enhanced
        var parts = version.Split('.');
        return parts.Length >= 2 && parts.Length <= 4 && 
               parts.All(part => int.TryParse(part, out var _));
    }

    public static implicit operator string(ProductVersion version) => version.Value;
    public static implicit operator ProductVersion(string value) => Create(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
