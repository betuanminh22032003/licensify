using MediatR;
using ProductService.Application.Commands.Products;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ProductService.Application.Commands.Products.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product with ID {ProductId}", request.Id);

        // Get existing product
        var productId = ProductId.Create(request.Id);
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        
        if (product == null)
        {
            throw new ProductNotFoundException(request.Id);
        }

        // Create value objects
        var productName = ProductName.Create(request.Name);
        var productVersion = ProductVersion.Create(request.Version);

        // Update product
        product.UpdateDetails(productName, productVersion, request.Description, Guid.Empty); // TODO: Get current user ID

        // Save changes
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product updated successfully with ID {ProductId}", product.Id.Value);

        return new UpdateProductResponse(
            product.Id.Value,
            product.Name.Value,
            product.Version.Value,
            product.Description,
            product.UpdatedAt
        );
    }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product with ID {ProductId}", request.Id);

        var productId = ProductId.Create(request.Id);
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        
        if (product == null)
        {
            throw new ProductNotFoundException(request.Id);
        }

        await _productRepository.DeleteAsync(productId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product deleted successfully with ID {ProductId}", request.Id);
        return true;
    }
}

public class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateProductCommandHandler> _logger;

    public ActivateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(ActivateProductCommand request, CancellationToken cancellationToken)
    {
        var productId = ProductId.Create(request.Id);
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        
        if (product == null)
        {
            throw new ProductNotFoundException(request.Id);
        }

        product.Activate(Guid.Empty); // TODO: Get current user ID
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateProductCommandHandler> _logger;

    public DeactivateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        var productId = ProductId.Create(request.Id);
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        
        if (product == null)
        {
            throw new ProductNotFoundException(request.Id);
        }

        product.Deactivate(Guid.Empty); // TODO: Get current user ID
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
