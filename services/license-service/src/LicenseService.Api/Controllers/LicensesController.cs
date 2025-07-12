using Microsoft.AspNetCore.Mvc;
using MediatR;
using LicenseService.Application.Commands.Licenses;
using LicenseService.Application.Queries.Licenses;
using LicenseService.Domain.Enums;

namespace LicenseService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LicensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LicensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateLicenseResponse>> CreateLicense(
        CreateLicenseCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetLicenseById), new { id = result.LicenseId }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LicenseDto>> GetLicenseById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLicenseByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("by-key/{licenseKey}")]
    public async Task<ActionResult<LicenseDto>> GetLicenseByKey(
        string licenseKey,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLicenseByKeyQuery(licenseKey);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IEnumerable<LicenseDto>>> GetLicensesByCustomer(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLicensesByCustomerQuery(customerId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<IEnumerable<LicenseDto>>> GetLicensesByProduct(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLicensesByProductQuery(productId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<LicenseDto>>> GetLicensesByStatus(
        LicenseStatus status,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLicensesByStatusQuery(status);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("expiring")]
    public async Task<ActionResult<IEnumerable<LicenseDto>>> GetExpiringLicenses(
        [FromQuery] DateTime? beforeDate = null,
        CancellationToken cancellationToken = default)
    {
        var date = beforeDate ?? DateTime.UtcNow.AddDays(30); // Default to 30 days
        var query = new GetExpiringLicensesQuery(date);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("expired")]
    public async Task<ActionResult<IEnumerable<LicenseDto>>> GetExpiredLicenses(
        CancellationToken cancellationToken = default)
    {
        var query = new GetExpiredLicensesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/activate")]
    public async Task<ActionResult> ActivateLicense(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new ActivateLicenseCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return BadRequest("Failed to activate license");

        return NoContent();
    }

    [HttpPut("{id:guid}/suspend")]
    public async Task<ActionResult> SuspendLicense(
        Guid id,
        [FromBody] SuspendLicenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new SuspendLicenseCommand(id, request.Reason);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return BadRequest("Failed to suspend license");

        return NoContent();
    }

    [HttpPut("{id:guid}/revoke")]
    public async Task<ActionResult> RevokeLicense(
        Guid id,
        [FromBody] RevokeLicenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new RevokeLicenseCommand(id, request.Reason);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return BadRequest("Failed to revoke license");

        return NoContent();
    }

    [HttpPut("{id:guid}/extend")]
    public async Task<ActionResult> ExtendLicense(
        Guid id,
        [FromBody] ExtendLicenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new ExtendLicenseCommand(id, request.NewExpirationDate);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return BadRequest("Failed to extend license");

        return NoContent();
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ValidateLicenseResponse>> ValidateLicense(
        [FromBody] ValidateLicenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new ValidateLicenseCommand(request.LicenseKey);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

// Request DTOs
public record SuspendLicenseRequest(string Reason);
public record RevokeLicenseRequest(string Reason);
public record ExtendLicenseRequest(DateTime NewExpirationDate);
public record ValidateLicenseRequest(string LicenseKey);
