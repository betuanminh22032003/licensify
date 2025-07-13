using MediatR;

namespace ProductService.Application.Commands.Products;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;

public record ActivateProductCommand(Guid Id) : IRequest<bool>;

public record DeactivateProductCommand(Guid Id) : IRequest<bool>;

public record ChangeProductVersionCommand(
    Guid Id,
    string NewVersion
) : IRequest<bool>;
