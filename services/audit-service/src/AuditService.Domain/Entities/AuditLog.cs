using AuditService.Domain.Common;
using AuditService.Domain.ValueObjects;
using AuditService.Domain.Enums;
using AuditService.Domain.Events;

namespace AuditService.Domain.Entities;

public class AuditLog : BaseEntity
{
    private AuditLog() { } // EF Constructor

    public AuditId Id { get; private set; } = null!;
    public UserId? UserId { get; private set; }
    public string? UserName { get; private set; }
    public EntityType EntityType { get; private set; }
    public string? EntityId { get; private set; }
    public AuditAction Action { get; private set; }
    public AuditSeverity Severity { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Source { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? AdditionalData { get; private set; }

    public static AuditLog Create(
        UserId? userId,
        string? userName,
        EntityType entityType,
        string? entityId,
        AuditAction action,
        string description,
        AuditSeverity severity = AuditSeverity.Information,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? source = null,
        string? additionalData = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        var auditLog = new AuditLog
        {
            Id = AuditId.NewId(),
            UserId = userId,
            UserName = userName,
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Severity = severity,
            Description = description,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Source = source,
            Timestamp = DateTime.UtcNow,
            AdditionalData = additionalData
        };

        auditLog.AddDomainEvent(new AuditLogCreatedEvent(auditLog.Id, auditLog.Action, auditLog.EntityType));
        return auditLog;
    }

    public static AuditLog CreateUserLoginAudit(
        UserId userId,
        string userName,
        string? ipAddress = null,
        string? userAgent = null,
        bool success = true)
    {
        var description = success ? $"User {userName} logged in successfully" : $"Failed login attempt for user {userName}";
        var severity = success ? AuditSeverity.Information : AuditSeverity.Warning;

        return Create(
            userId,
            userName,
            EntityType.User,
            userId.ToString(),
            AuditAction.Login,
            description,
            severity,
            ipAddress: ipAddress,
            userAgent: userAgent,
            source: "AuthService");
    }

    public static AuditLog CreateUserLogoutAudit(
        UserId userId,
        string userName,
        string? ipAddress = null)
    {
        return Create(
            userId,
            userName,
            EntityType.User,
            userId.ToString(),
            AuditAction.Logout,
            $"User {userName} logged out",
            ipAddress: ipAddress,
            source: "AuthService");
    }

    public static AuditLog CreateLicenseAudit(
        UserId? userId,
        string? userName,
        string licenseId,
        AuditAction action,
        string description,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null)
    {
        return Create(
            userId,
            userName,
            EntityType.License,
            licenseId,
            action,
            description,
            oldValues: oldValues,
            newValues: newValues,
            ipAddress: ipAddress,
            source: "LicenseService");
    }

    public static AuditLog CreateProductAudit(
        UserId userId,
        string userName,
        string productId,
        AuditAction action,
        string description,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null)
    {
        return Create(
            userId,
            userName,
            EntityType.Product,
            productId,
            action,
            description,
            oldValues: oldValues,
            newValues: newValues,
            ipAddress: ipAddress,
            source: "ProductService");
    }

    public static AuditLog CreateSystemAudit(
        string description,
        AuditSeverity severity = AuditSeverity.Information,
        string? additionalData = null)
    {
        return Create(
            null,
            "System",
            EntityType.System,
            null,
            AuditAction.Configure,
            description,
            severity,
            source: "System",
            additionalData: additionalData);
    }

    public bool IsHighSeverity() => Severity >= AuditSeverity.Error;

    public bool IsSecurityRelevant() => Action switch
    {
        AuditAction.Login or AuditAction.Logout or AuditAction.Access => true,
        _ => false
    };
}
