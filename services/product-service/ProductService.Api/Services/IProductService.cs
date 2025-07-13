using Licensify.Shared.DTOs;

namespace ProductService.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(ProductSearchRequest request);
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request, Guid userId);
    Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductRequest request, Guid userId);
    Task<bool> DeleteProductAsync(Guid id);
    Task<bool> ProductExistsAsync(Guid id);
}
