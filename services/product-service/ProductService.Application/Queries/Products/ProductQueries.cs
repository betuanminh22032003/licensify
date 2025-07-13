using MediatR;

namespace ProductService.Application.Queries.Products;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;

public record GetAllProductsQuery(
    bool? IsActive = null
) : IRequest<IEnumerable<ProductDto>>;

public record SearchProductsQuery(
    string? Name = null,
    string? Version = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<ProductDto>>;

public record ProductDto(
    Guid Id,
    string Name,
    string Version,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
