using AuthService.Domain.Common;

namespace AuthService.Domain.ValueObjects;

public class UserRole : ValueObject
{
    public string Value { get; private set; }

    private UserRole(string value)
    {
        Value = value;
    }

    public static UserRole Admin => new("Admin");
    public static UserRole Developer => new("Developer");

    public static UserRole Create(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        var normalizedRole = role.Trim();

        if (!IsValidRole(normalizedRole))
            throw new ArgumentException($"Invalid role: {role}. Valid roles are: Admin, Developer", nameof(role));

        return new UserRole(normalizedRole);
    }

    private static bool IsValidRole(string role)
    {
        return role == "Admin" || role == "Developer";
    }

    public bool IsAdmin => Value == "Admin";
    public bool IsDeveloper => Value == "Developer";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(UserRole role) => role.Value;
}
