using MediatR;
using LicenseService.Application.Queries.Licenses;
using LicenseService.Domain.Repositories;
using LicenseService.Domain.ValueObjects;
using LicenseService.Domain.Entities;

namespace LicenseService.Application.Queries.Licenses.Handlers;

public class GetLicenseByIdQueryHandler : IRequestHandler<GetLicenseByIdQuery, LicenseDto?>
{
    private readonly ILicenseRepository _licenseRepository;

    public GetLicenseByIdQueryHandler(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<LicenseDto?> Handle(GetLicenseByIdQuery request, CancellationToken cancellationToken)
    {
        var licenseId = LicenseId.Create(request.LicenseId);
        var license = await _licenseRepository.GetByIdAsync(licenseId, cancellationToken);

        return license != null ? MapToDto(license) : null;
    }

    private static LicenseDto MapToDto(License license)
    {
        return new LicenseDto
        {
            Id = license.Id,
            ProductId = license.ProductId,
            CustomerId = license.CustomerId,
            LicenseKey = license.LicenseKey,
            Type = license.Type,
            Status = license.Status,
            IssuedAt = license.IssuedAt,
            ExpiresAt = license.ExpiresAt,
            ActivatedAt = license.ActivatedAt,
            LastValidatedAt = license.LastValidatedAt,
            MaxUsers = license.MaxUsers,
            CurrentUsers = license.CurrentUsers,
            Notes = license.Notes,
            CreatedAt = license.CreatedAt,
            UpdatedAt = license.UpdatedAt
        };
    }
}

public class GetLicenseByKeyQueryHandler : IRequestHandler<GetLicenseByKeyQuery, LicenseDto?>
{
    private readonly ILicenseRepository _licenseRepository;

    public GetLicenseByKeyQueryHandler(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<LicenseDto?> Handle(GetLicenseByKeyQuery request, CancellationToken cancellationToken)
    {
        var licenseKey = LicenseKey.Create(request.LicenseKey);
        var license = await _licenseRepository.GetByLicenseKeyAsync(licenseKey, cancellationToken);

        return license != null ? MapToDto(license) : null;
    }

    private static LicenseDto MapToDto(License license)
    {
        return new LicenseDto
        {
            Id = license.Id,
            ProductId = license.ProductId,
            CustomerId = license.CustomerId,
            LicenseKey = license.LicenseKey,
            Type = license.Type,
            Status = license.Status,
            IssuedAt = license.IssuedAt,
            ExpiresAt = license.ExpiresAt,
            ActivatedAt = license.ActivatedAt,
            LastValidatedAt = license.LastValidatedAt,
            MaxUsers = license.MaxUsers,
            CurrentUsers = license.CurrentUsers,
            Notes = license.Notes,
            CreatedAt = license.CreatedAt,
            UpdatedAt = license.UpdatedAt
        };
    }
}

public class GetLicensesByCustomerQueryHandler : IRequestHandler<GetLicensesByCustomerQuery, IEnumerable<LicenseDto>>
{
    private readonly ILicenseRepository _licenseRepository;

    public GetLicensesByCustomerQueryHandler(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<IEnumerable<LicenseDto>> Handle(GetLicensesByCustomerQuery request, CancellationToken cancellationToken)
    {
        var customerId = CustomerId.Create(request.CustomerId);
        var licenses = await _licenseRepository.GetByCustomerIdAsync(customerId, cancellationToken);

        return licenses.Select(MapToDto);
    }

    private static LicenseDto MapToDto(License license)
    {
        return new LicenseDto
        {
            Id = license.Id,
            ProductId = license.ProductId,
            CustomerId = license.CustomerId,
            LicenseKey = license.LicenseKey,
            Type = license.Type,
            Status = license.Status,
            IssuedAt = license.IssuedAt,
            ExpiresAt = license.ExpiresAt,
            ActivatedAt = license.ActivatedAt,
            LastValidatedAt = license.LastValidatedAt,
            MaxUsers = license.MaxUsers,
            CurrentUsers = license.CurrentUsers,
            Notes = license.Notes,
            CreatedAt = license.CreatedAt,
            UpdatedAt = license.UpdatedAt
        };
    }
}

public class GetLicensesByProductQueryHandler : IRequestHandler<GetLicensesByProductQuery, IEnumerable<LicenseDto>>
{
    private readonly ILicenseRepository _licenseRepository;

    public GetLicensesByProductQueryHandler(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<IEnumerable<LicenseDto>> Handle(GetLicensesByProductQuery request, CancellationToken cancellationToken)
    {
        var productId = ProductId.Create(request.ProductId);
        var licenses = await _licenseRepository.GetByProductIdAsync(productId, cancellationToken);

        return licenses.Select(MapToDto);
    }

    private static LicenseDto MapToDto(License license)
    {
        return new LicenseDto
        {
            Id = license.Id,
            ProductId = license.ProductId,
            CustomerId = license.CustomerId,
            LicenseKey = license.LicenseKey,
            Type = license.Type,
            Status = license.Status,
            IssuedAt = license.IssuedAt,
            ExpiresAt = license.ExpiresAt,
            ActivatedAt = license.ActivatedAt,
            LastValidatedAt = license.LastValidatedAt,
            MaxUsers = license.MaxUsers,
            CurrentUsers = license.CurrentUsers,
            Notes = license.Notes,
            CreatedAt = license.CreatedAt,
            UpdatedAt = license.UpdatedAt
        };
    }
}

public class GetLicensesByStatusQueryHandler : IRequestHandler<GetLicensesByStatusQuery, IEnumerable<LicenseDto>>
{
    private readonly ILicenseRepository _licenseRepository;

    public GetLicensesByStatusQueryHandler(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<IEnumerable<LicenseDto>> Handle(GetLicensesByStatusQuery request, CancellationToken cancellationToken)
    {
        var licenses = await _licenseRepository.GetByStatusAsync(request.Status, cancellationToken);

        return licenses.Select(MapToDto);
    }

    private static LicenseDto MapToDto(License license)
    {
        return new LicenseDto
        {
            Id = license.Id,
            ProductId = license.ProductId,
            CustomerId = license.CustomerId,
            LicenseKey = license.LicenseKey,
            Type = license.Type,
            Status = license.Status,
            IssuedAt = license.IssuedAt,
            ExpiresAt = license.ExpiresAt,
            ActivatedAt = license.ActivatedAt,
            LastValidatedAt = license.LastValidatedAt,
            MaxUsers = license.MaxUsers,
            CurrentUsers = license.CurrentUsers,
            Notes = license.Notes,
            CreatedAt = license.CreatedAt,
            UpdatedAt = license.UpdatedAt
        };
    }
}

public class GetExpiringLicensesQueryHandler : IRequestHandler<GetExpiringLicensesQuery, IEnumerable<LicenseDto>>
{
    private readonly ILicenseRepository _licenseRepository;

    public GetExpiringLicensesQueryHandler(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<IEnumerable<LicenseDto>> Handle(GetExpiringLicensesQuery request, CancellationToken cancellationToken)
    {
        var licenses = await _licenseRepository.GetExpiringLicensesAsync(request.BeforeDate, cancellationToken);

        return licenses.Select(MapToDto);
    }

    private static LicenseDto MapToDto(License license)
    {
        return new LicenseDto
        {
            Id = license.Id,
            ProductId = license.ProductId,
            CustomerId = license.CustomerId,
            LicenseKey = license.LicenseKey,
            Type = license.Type,
            Status = license.Status,
            IssuedAt = license.IssuedAt,
            ExpiresAt = license.ExpiresAt,
            ActivatedAt = license.ActivatedAt,
            LastValidatedAt = license.LastValidatedAt,
            MaxUsers = license.MaxUsers,
            CurrentUsers = license.CurrentUsers,
            Notes = license.Notes,
            CreatedAt = license.CreatedAt,
            UpdatedAt = license.UpdatedAt
        };
    }
}

public class GetExpiredLicensesQueryHandler : IRequestHandler<GetExpiredLicensesQuery, IEnumerable<LicenseDto>>
{
    private readonly ILicenseRepository _licenseRepository;

    public GetExpiredLicensesQueryHandler(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<IEnumerable<LicenseDto>> Handle(GetExpiredLicensesQuery request, CancellationToken cancellationToken)
    {
        var licenses = await _licenseRepository.GetExpiredLicensesAsync(cancellationToken);

        return licenses.Select(MapToDto);
    }

    private static LicenseDto MapToDto(License license)
    {
        return new LicenseDto
        {
            Id = license.Id,
            ProductId = license.ProductId,
            CustomerId = license.CustomerId,
            LicenseKey = license.LicenseKey,
            Type = license.Type,
            Status = license.Status,
            IssuedAt = license.IssuedAt,
            ExpiresAt = license.ExpiresAt,
            ActivatedAt = license.ActivatedAt,
            LastValidatedAt = license.LastValidatedAt,
            MaxUsers = license.MaxUsers,
            CurrentUsers = license.CurrentUsers,
            Notes = license.Notes,
            CreatedAt = license.CreatedAt,
            UpdatedAt = license.UpdatedAt
        };
    }
}
