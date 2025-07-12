using AuditService.Domain.Common;

namespace AuditService.Domain.ValueObjects;

public class AuditId : ValueObject
{
    public Guid Value { get; private set; }

    private AuditId(Guid value)
    {
        Value = value;
    }

    public static AuditId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AuditId cannot be empty", nameof(value));

        return new AuditId(value);
    }

    public static AuditId NewId() => new(Guid.NewGuid());

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(AuditId auditId) => auditId.Value;
    public static implicit operator AuditId(Guid value) => Create(value);
}
