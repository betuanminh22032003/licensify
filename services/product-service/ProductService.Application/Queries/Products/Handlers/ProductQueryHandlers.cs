using MediatR;
using ProductService.Application.Queries.Products;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ProductService.Application.Queries.Products.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product with ID {ProductId}", request.Id);

        var productId = ProductId.Create(request.Id);
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
            return null;
        }

        return new ProductDto(
            product.Id.Value,
            product.Name.Value,
            product.Version.Value,
            product.Description,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt
        );
    }
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(
        IProductRepository productRepository,
        ILogger<GetAllProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all products with IsActive filter: {IsActive}", request.IsActive);

        var products = request.IsActive.HasValue && request.IsActive.Value
            ? await _productRepository.GetActiveProductsAsync(cancellationToken)
            : await _productRepository.GetAllAsync(cancellationToken);

        return products.Select(product => new ProductDto(
            product.Id.Value,
            product.Name.Value,
            product.Version.Value,
            product.Description,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt
        ));
    }
}

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SearchProductsQueryHandler> _logger;

    public SearchProductsQueryHandler(
        IProductRepository productRepository,
        ILogger<SearchProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching products with Name: {Name}, Version: {Version}, IsActive: {IsActive}", 
            request.Name, request.Version, request.IsActive);

        var products = await _productRepository.SearchAsync(
            request.Name, 
            request.Version, 
            request.IsActive, 
            cancellationToken);

        var productList = products.ToList();
        var totalCount = productList.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedProducts = productList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(product => new ProductDto(
                product.Id.Value,
                product.Name.Value,
                product.Version.Value,
                product.Description,
                product.IsActive,
                product.CreatedAt,
                product.UpdatedAt
            ));

        return new PagedResult<ProductDto>(
            pagedProducts,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}
