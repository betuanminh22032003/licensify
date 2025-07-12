using AuditService.Domain.Entities;
using AuditService.Domain.ValueObjects;
using AuditService.Domain.Enums;

namespace AuditService.Domain.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(AuditId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(EntityType entityType, string entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByActionAsync(AuditAction action, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetBySeverityAsync(AuditSeverity severity, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetSecurityAuditsAsync(DateTime? startDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetRecentAuditsAsync(int count = 100, CancellationToken cancellationToken = default);
    Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<int> GetCountByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task DeleteOldAuditsAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}
