using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.Products;
using ProductService.Application.Queries.Products;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] bool? isActive = null)
    {
        try
        {
            var query = new GetAllProductsQuery(isActive);
            var products = await _mediator.Send(query);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        try
        {
            var query = new GetProductByIdQuery(id);
            var product = await _mediator.Send(query);
            
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Search products
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<ProductDto>>> SearchProducts(
        [FromQuery] string? name = null,
        [FromQuery] string? version = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new SearchProductsQuery(name, version, isActive, page, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateProductResponse>> CreateProduct([FromBody] CreateProductCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateProductResponse>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var command = new UpdateProductCommand(id, request.Name, request.Version, request.Description);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Activate a product
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult> ActivateProduct(Guid id)
    {
        try
        {
            var command = new ActivateProductCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(new { message = "Product activated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating product {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Deactivate a product
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult> DeactivateProduct(Guid id)
    {
        try
        {
            var command = new DeactivateProductCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(new { message = "Product deactivated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating product {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Change product version
    /// </summary>
    [HttpPost("{id:guid}/change-version")]
    public async Task<ActionResult> ChangeProductVersion(Guid id, [FromBody] ChangeVersionRequest request)
    {
        try
        {
            var command = new ChangeProductVersionCommand(id, request.NewVersion);
            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(new { message = "Product version changed successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing version for product {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

public record UpdateProductRequest(string Name, string Version, string? Description);
public record ChangeVersionRequest(string NewVersion);
