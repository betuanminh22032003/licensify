using Microsoft.EntityFrameworkCore;
using LicenseService.Domain.Entities;
using LicenseService.Domain.Repositories;
using LicenseService.Domain.ValueObjects;
using LicenseService.Domain.Enums;
using LicenseService.Infrastructure.Persistence;

namespace LicenseService.Infrastructure.Repositories;

public class LicenseRepository : ILicenseRepository
{
    private readonly LicenseDbContext _context;

    public LicenseRepository(LicenseDbContext context)
    {
        _context = context;
    }

    public async Task<License?> GetByIdAsync(LicenseId id, CancellationToken cancellationToken = default)
    {
        return await _context.Licenses
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<License?> GetByLicenseKeyAsync(LicenseKey licenseKey, CancellationToken cancellationToken = default)
    {
        return await _context.Licenses
            .FirstOrDefaultAsync(l => l.LicenseKey == licenseKey, cancellationToken);
    }

    public async Task<IEnumerable<License>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Licenses
            .Where(l => l.CustomerId == customerId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<License>> GetByProductIdAsync(ProductId productId, CancellationToken cancellationToken = default)
    {
        return await _context.Licenses
            .Where(l => l.ProductId == productId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<License>> GetByStatusAsync(LicenseStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Licenses
            .Where(l => l.Status == status)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<License>> GetExpiringLicensesAsync(DateTime beforeDate, CancellationToken cancellationToken = default)
    {
        return await _context.Licenses
            .Where(l => l.Status == LicenseStatus.Active && l.ExpiresAt <= beforeDate)
            .OrderBy(l => l.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<License>> GetExpiredLicensesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Licenses
            .Where(l => l.ExpiresAt < now && l.Status != LicenseStatus.Expired)
            .OrderBy(l => l.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<License> AddAsync(License license, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Licenses.AddAsync(license, cancellationToken);
        return entry.Entity;
    }

    public void Update(License license)
    {
        _context.Licenses.Update(license);
    }

    public void Delete(License license)
    {
        _context.Licenses.Remove(license);
    }

    public async Task<bool> ExistsByLicenseKeyAsync(LicenseKey licenseKey, CancellationToken cancellationToken = default)
    {
        return await _context.Licenses
            .AnyAsync(l => l.LicenseKey == licenseKey, cancellationToken);
    }

    public async Task<int> GetActiveLicenseCountByProductAsync(ProductId productId, CancellationToken cancellationToken = default)
    {
        return await _context.Licenses
            .CountAsync(l => l.ProductId == productId && l.Status == LicenseStatus.Active, cancellationToken);
    }
}
