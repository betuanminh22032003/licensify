using AuditService.Infrastructure.Data;
using AuditService.Application.Interfaces;

namespace AuditService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuditDbContext _context;

    public UnitOfWork(AuditDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
