using LicenseService.Domain.Common;

namespace LicenseService.Domain.ValueObjects;

public class LicenseId : ValueObject
{
    public Guid Value { get; private set; }

    private LicenseId(Guid value)
    {
        Value = value;
    }

    public static LicenseId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("LicenseId cannot be empty", nameof(value));

        return new LicenseId(value);
    }

    public static LicenseId NewId() => new(Guid.NewGuid());

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(LicenseId licenseId) => licenseId.Value;
    public static implicit operator LicenseId(Guid value) => Create(value);
}
