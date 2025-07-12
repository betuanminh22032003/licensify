using AuditService.Domain.Entities;
using AuditService.Domain.Enums;

namespace AuditService.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, string entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByActionAsync(AuditAction action, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
