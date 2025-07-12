using AuditService.Domain.Enums;

namespace AuditService.Application.DTOs;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public EntityType EntityType { get; set; }
    public string? EntityId { get; set; }
    public AuditAction Action { get; set; }
    public AuditSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Source { get; set; }
    public DateTime Timestamp { get; set; }
    public string? AdditionalData { get; set; }
}
