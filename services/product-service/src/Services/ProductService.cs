using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using Licensify.Shared.DTOs;

namespace ProductService.Services;

public class ProductService : IProductService
{
    private readonly ProductDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ProductDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductSearchRequest request)
    {
        try
        {
            var query = _context.Products.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(p => p.Name.Contains(request.Name));
            }

            if (!string.IsNullOrEmpty(request.Version))
            {
                query = query.Where(p => p.Version.Contains(request.Version));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var products = await query
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Version)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => MapToProductDto(p))
                .ToListAsync();

            return new PagedResult<ProductDto>(
                products,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            throw;
        }
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        try
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            return product != null ? MapToProductDto(product) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by ID: {ProductId}", id);
            return null;
        }
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request, Guid userId)
    {
        try
        {
            // Check if product with same name and version exists
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == request.Name && p.Version == request.Version && p.IsActive);

            if (existingProduct != null)
            {
                throw new InvalidOperationException($"Product with name '{request.Name}' and version '{request.Version}' already exists");
            }

            var product = new Product
            {
                Name = request.Name,
                Version = request.Version,
                Description = request.Description,
                CreatedBy = userId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product created successfully: {ProductName} v{Version}", request.Name, request.Version);

            return MapToProductDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
            throw;
        }
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductRequest request, Guid userId)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive)
            {
                return null;
            }

            // Check if another product with same name and version exists
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id != id && p.Name == request.Name && p.Version == request.Version && p.IsActive);

            if (existingProduct != null)
            {
                throw new InvalidOperationException($"Another product with name '{request.Name}' and version '{request.Version}' already exists");
            }

            product.Name = request.Name;
            product.Version = request.Version;
            product.Description = request.Description;
            product.UpdatedBy = userId;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Product updated successfully: {ProductId}", id);

            return MapToProductDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            // Soft delete
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Product soft deleted: {ProductId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product: {ProductId}", id);
            return false;
        }
    }

    public async Task<bool> ProductExistsAsync(Guid id)
    {
        try
        {
            return await _context.Products.AnyAsync(p => p.Id == id && p.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if product exists: {ProductId}", id);
            return false;
        }
    }

    private static ProductDto MapToProductDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Version,
            product.Description,
            product.CreatedAt,
            product.UpdatedAt
        );
    }
}
