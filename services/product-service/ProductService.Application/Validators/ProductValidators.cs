using FluentValidation;
using ProductService.Application.Commands.Products;
using ProductService.Application.Queries.Products;

namespace ProductService.Application.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .Length(1, 100).WithMessage("Product name must be between 1 and 100 characters");

        RuleFor(x => x.Version)
            .NotEmpty().WithMessage("Product version is required")
            .Length(1, 50).WithMessage("Product version must be between 1 and 50 characters")
            .Matches(@"^\d+(\.\d+){1,3}$").WithMessage("Version must be in format x.x.x or x.x.x.x");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot be longer than 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .Length(1, 100).WithMessage("Product name must be between 1 and 100 characters");

        RuleFor(x => x.Version)
            .NotEmpty().WithMessage("Product version is required")
            .Length(1, 50).WithMessage("Product version must be between 1 and 50 characters")
            .Matches(@"^\d+(\.\d+){1,3}$").WithMessage("Version must be in format x.x.x or x.x.x.x");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot be longer than 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize must be greater than or equal to 1")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot be greater than 100");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name search term cannot be longer than 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Version)
            .MaximumLength(50).WithMessage("Version search term cannot be longer than 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Version));
    }
}
