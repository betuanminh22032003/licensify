using MediatR;
using AuditService.Domain.Enums;

namespace AuditService.Application.Queries.Audit;

public record GetAuditLogByIdQuery(Guid AuditId) : IRequest<AuditLogDto?>;

public record GetAllAuditLogsQuery(int Skip = 0, int Take = 100) : IRequest<IEnumerable<AuditLogDto>>;

public record GetAuditLogsByUserQuery(Guid UserId) : IRequest<IEnumerable<AuditLogDto>>;

public record GetAuditLogsByEntityQuery(EntityType EntityType, string EntityId) : IRequest<IEnumerable<AuditLogDto>>;

public record GetAuditLogsByActionQuery(AuditAction Action) : IRequest<IEnumerable<AuditLogDto>>;

public record GetAuditLogsBySeverityQuery(AuditSeverity Severity) : IRequest<IEnumerable<AuditLogDto>>;

public record GetAuditLogsByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<IEnumerable<AuditLogDto>>;

public record GetSecurityAuditsQuery(DateTime? StartDate = null) : IRequest<IEnumerable<AuditLogDto>>;

public record GetRecentAuditsQuery(int Count = 100) : IRequest<IEnumerable<AuditLogDto>>;

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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
