using AuthService.Domain.Common;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Events;

namespace AuthService.Domain.Entities;

public class User : BaseEntity
{
    private User() { } // Private constructor for EF Core

    public UserId Id { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }

    // Navigation properties
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public static User Create(
        Email email,
        string username,
        string password,
        string firstName,
        string lastName,
        UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        var user = new User
        {
            Id = UserId.NewId(),
            Email = email,
            Username = username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Role = role,
            IsActive = true
        };

        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email));
        return user;
    }

    public bool VerifyPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    public void ChangePassword(string newPassword)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot change password for inactive user");

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Password cannot be empty", nameof(newPassword));

        PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        UpdateTimestamp();

        AddDomainEvent(new UserPasswordChangedEvent(Id));
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot update profile for inactive user");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        UpdateTimestamp();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, FirstName, LastName));
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        UpdateTimestamp();

        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        UpdateTimestamp();

        AddDomainEvent(new UserActivatedEvent(Id));
    }

    public RefreshToken CreateRefreshToken(string token, DateTime expiresAt)
    {
        var refreshToken = RefreshToken.Create(Id, token, expiresAt);
        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public void RevokeRefreshToken(Guid refreshTokenId)
    {
        var token = _refreshTokens.FirstOrDefault(rt => rt.Id == refreshTokenId);
        token?.Revoke();
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(rt => !rt.IsRevoked))
        {
            token.Revoke();
        }

        AddDomainEvent(new UserAllTokensRevokedEvent(Id));
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdateTimestamp();
    }
}
