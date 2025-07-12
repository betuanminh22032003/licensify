using AuditService.Application.Interfaces;
using AuditService.Domain.Entities;
using AuditService.Domain.Enums;
using AuditService.Domain.ValueObjects;
using AuditService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AuditDbContext _context;

    public AuditLogRepository(AuditDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        var entry = await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        return entry.Entity;
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, string entityId, CancellationToken cancellationToken = default)
    {
        if (Enum.TryParse<EntityType>(entityType, out var entityTypeEnum))
        {
            return await _context.AuditLogs
                .Where(x => x.EntityType == entityTypeEnum && x.EntityId == entityId)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync(cancellationToken);
        }
        return Enumerable.Empty<AuditLog>();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(userId, out var userGuid))
        {
            var userIdValue = UserId.Create(userGuid);
            return await _context.AuditLogs
                .Where(x => x.UserId == userIdValue)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync(cancellationToken);
        }
        return Enumerable.Empty<AuditLog>();
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(AuditAction action, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(x => x.Action == action)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .OrderByDescending(x => x.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs.CountAsync(cancellationToken);
    }
}
