using MediatR;

namespace ProductService.Application.Commands.Products;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Version,
    string? Description = null
) : IRequest<UpdateProductResponse>;

public record UpdateProductResponse(
    Guid Id,
    string Name,
    string Version,
    string? Description,
    DateTime UpdatedAt
);
