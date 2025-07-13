using MediatR;
using ProductService.Application.Commands.Products;
using ProductService.Domain.Entities;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ProductService.Application.Commands.Products.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product with name {Name} and version {Version}", request.Name, request.Version);

        // Create value objects
        var productName = ProductName.Create(request.Name);
        var productVersion = ProductVersion.Create(request.Version);

        // Check if product already exists
        var existingProduct = await _productRepository.GetByNameAndVersionAsync(productName, productVersion, cancellationToken);
        if (existingProduct != null)
        {
            throw new ProductAlreadyExistsException(request.Name, request.Version);
        }

        // Create new product
        var product = Product.Create(productName, productVersion, request.Description);

        // Save to repository
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product created successfully with ID {ProductId}", product.Id.Value);

        return new CreateProductResponse(
            product.Id.Value,
            product.Name.Value,
            product.Version.Value,
            product.Description,
            product.IsActive,
            product.CreatedAt
        );
    }
}
