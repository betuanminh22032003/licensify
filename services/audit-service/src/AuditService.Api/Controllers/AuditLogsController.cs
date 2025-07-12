using AuditService.Application.Commands.Audit;
using AuditService.Application.Queries.Audit;
using AuditService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuditService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new audit log entry
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateAuditLogResponse>> CreateAuditLog([FromBody] CreateAuditLogCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAuditLogById), new { id = result.AuditId }, result);
    }

    /// <summary>
    /// Get audit log by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuditLogDto>> GetAuditLogById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAuditLogByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Get all audit logs with pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAllAuditLogs(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 100, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllAuditLogsQuery(skip, take);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get audit logs by entity type and ID
    /// </summary>
    [HttpGet("entity/{entityType}/{entityId}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByEntity(
        string entityType, 
        string entityId, 
        CancellationToken cancellationToken)
    {
        if (Enum.TryParse<EntityType>(entityType, out var entityTypeEnum))
        {
            var query = new GetAuditLogsByEntityQuery(entityTypeEnum, entityId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        return BadRequest("Invalid entity type");
    }

    /// <summary>
    /// Get audit logs by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByUser(
        string userId, 
        CancellationToken cancellationToken)
    {
        if (Guid.TryParse(userId, out var userGuid))
        {
            var query = new GetAuditLogsByUserQuery(userGuid);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        return BadRequest("Invalid user ID format");
    }

    /// <summary>
    /// Get audit logs by action
    /// </summary>
    [HttpGet("action/{action}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByAction(
        AuditAction action, 
        CancellationToken cancellationToken)
    {
        var query = new GetAuditLogsByActionQuery(action);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get audit logs by date range
    /// </summary>
    [HttpGet("daterange")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate, 
        CancellationToken cancellationToken)
    {
        var query = new GetAuditLogsByDateRangeQuery(startDate, endDate);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
