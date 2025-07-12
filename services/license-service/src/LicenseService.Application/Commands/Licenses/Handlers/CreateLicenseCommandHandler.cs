using MediatR;
using LicenseService.Application.Commands.Licenses;
using LicenseService.Domain.Entities;
using LicenseService.Domain.Repositories;
using LicenseService.Domain.ValueObjects;
using LicenseService.Domain.Exceptions;

namespace LicenseService.Application.Commands.Licenses.Handlers;

public class CreateLicenseCommandHandler : IRequestHandler<CreateLicenseCommand, CreateLicenseResponse>
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLicenseCommandHandler(
        ILicenseRepository licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateLicenseResponse> Handle(CreateLicenseCommand request, CancellationToken cancellationToken)
    {
        var productId = ProductId.Create(request.ProductId);
        var customerId = CustomerId.Create(request.CustomerId);

        var license = License.Create(
            productId,
            customerId,
            request.Type,
            request.ExpiresAt,
            request.MaxUsers,
            request.Notes);

        await _licenseRepository.AddAsync(license, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateLicenseResponse(
            license.Id,
            license.LicenseKey,
            license.IssuedAt,
            license.ExpiresAt
        );
    }
}
