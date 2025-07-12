namespace Licensify.Shared.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Version,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateProductRequest(
    string Name,
    string Version,
    string? Description = null
);

public record UpdateProductRequest(
    string Name,
    string Version,
    string? Description = null
);

public record ProductSearchRequest(
    string? Name = null,
    string? Version = null,
    int Page = 1,
    int PageSize = 10
);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
