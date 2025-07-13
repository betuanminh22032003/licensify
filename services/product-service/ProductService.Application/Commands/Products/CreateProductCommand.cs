using MediatR;

namespace ProductService.Application.Commands.Products;

public record CreateProductCommand(
    string Name,
    string Version,
    string? Description = null
) : IRequest<CreateProductResponse>;

public record CreateProductResponse(
    Guid Id,
    string Name,
    string Version,
    string? Description,
    bool IsActive,
    DateTime CreatedAt
);
