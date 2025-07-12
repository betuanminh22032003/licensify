using LicenseService.Domain.Common;

namespace LicenseService.Domain.ValueObjects;

public class LicenseKey : ValueObject
{
    public string Value { get; private set; }

    private LicenseKey(string value)
    {
        Value = value;
    }

    public static LicenseKey Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("License key cannot be empty", nameof(value));

        if (value.Length < 20 || value.Length > 100)
            throw new ArgumentException("License key must be between 20 and 100 characters", nameof(value));

        return new LicenseKey(value);
    }

    public static LicenseKey Generate()
    {
        // Generate a secure license key
        var key = Guid.NewGuid().ToString("N").ToUpper() + 
                  Guid.NewGuid().ToString("N").ToUpper()[..8];
        return new LicenseKey(key);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(LicenseKey licenseKey) => licenseKey.Value;
}
