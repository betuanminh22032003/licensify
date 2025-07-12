using AuthService.Domain.Common;

namespace AuthService.Domain.ValueObjects;

public class UserId : ValueObject
{
    public Guid Value { get; private set; }

    // EF Core constructor
    public UserId(Guid value)
    {
        Value = value;
    }

    public static UserId Create(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(id));

        return new UserId(id);
    }

    public static UserId NewId() => new(Guid.NewGuid());

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static implicit operator UserId(Guid id) => Create(id);
}
