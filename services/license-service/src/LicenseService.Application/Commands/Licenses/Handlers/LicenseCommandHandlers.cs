using MediatR;
using LicenseService.Application.Commands.Licenses;
using LicenseService.Domain.Repositories;
using LicenseService.Domain.ValueObjects;
using LicenseService.Domain.Exceptions;

namespace LicenseService.Application.Commands.Licenses.Handlers;

public class ActivateLicenseCommandHandler : IRequestHandler<ActivateLicenseCommand, bool>
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateLicenseCommandHandler(
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ActivateLicenseCommand request, CancellationToken cancellationToken)
    {
        var licenseId = LicenseId.Create(request.LicenseId);
        var license = await _licenseRepository.GetByIdAsync(licenseId, cancellationToken);

        if (license == null)
            throw new LicenseNotFoundException(request.LicenseId);

        license.Activate();

        _licenseRepository.Update(license);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class SuspendLicenseCommandHandler : IRequestHandler<SuspendLicenseCommand, bool>
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SuspendLicenseCommandHandler(
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SuspendLicenseCommand request, CancellationToken cancellationToken)
    {
        var licenseId = LicenseId.Create(request.LicenseId);
        var license = await _licenseRepository.GetByIdAsync(licenseId, cancellationToken);

        if (license == null)
            throw new LicenseNotFoundException(request.LicenseId);

        license.Suspend(request.Reason);

        _licenseRepository.Update(license);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class RevokeLicenseCommandHandler : IRequestHandler<RevokeLicenseCommand, bool>
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeLicenseCommandHandler(
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RevokeLicenseCommand request, CancellationToken cancellationToken)
    {
        var licenseId = LicenseId.Create(request.LicenseId);
        var license = await _licenseRepository.GetByIdAsync(licenseId, cancellationToken);

        if (license == null)
            throw new LicenseNotFoundException(request.LicenseId);

        license.Revoke(request.Reason);

        _licenseRepository.Update(license);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class ExtendLicenseCommandHandler : IRequestHandler<ExtendLicenseCommand, bool>
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExtendLicenseCommandHandler(
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ExtendLicenseCommand request, CancellationToken cancellationToken)
    {
        var licenseId = LicenseId.Create(request.LicenseId);
        var license = await _licenseRepository.GetByIdAsync(licenseId, cancellationToken);

        if (license == null)
            throw new LicenseNotFoundException(request.LicenseId);

        license.ExtendExpiration(request.NewExpirationDate);

        _licenseRepository.Update(license);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class ValidateLicenseCommandHandler : IRequestHandler<ValidateLicenseCommand, ValidateLicenseResponse>
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ValidateLicenseCommandHandler(
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ValidateLicenseResponse> Handle(ValidateLicenseCommand request, CancellationToken cancellationToken)
    {
        var licenseKey = LicenseKey.Create(request.LicenseKey);
        var license = await _licenseRepository.GetByLicenseKeyAsync(licenseKey, cancellationToken);

        if (license == null)
        {
            return new ValidateLicenseResponse(false, "License not found");
        }

        var isValid = license.Validate();

        // Save validation timestamp
        _licenseRepository.Update(license);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!isValid)
        {
            var reason = license.IsExpired() ? "License has expired" : "License is not active";
            return new ValidateLicenseResponse(false, reason, license.ExpiresAt);
        }

        var remainingUsers = license.MaxUsers - license.CurrentUsers;
        return new ValidateLicenseResponse(true, null, license.ExpiresAt, remainingUsers);
    }
}
