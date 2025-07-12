using MediatR;
using AuditService.Application.Queries.Audit;
using AuditService.Domain.Repositories;
using AuditService.Domain.ValueObjects;
using AuditService.Domain.Entities;

namespace AuditService.Application.Queries.Audit.Handlers;

internal static class AuditLogMapper
{
    internal static AuditLogDto MapToDto(AuditLog auditLog)
    {
        return new AuditLogDto
        {
            Id = auditLog.Id,
            UserId = auditLog.UserId?.Value,
            UserName = auditLog.UserName,
            EntityType = auditLog.EntityType,
            EntityId = auditLog.EntityId,
            Action = auditLog.Action,
            Severity = auditLog.Severity,
            Description = auditLog.Description,
            OldValues = auditLog.OldValues,
            NewValues = auditLog.NewValues,
            IpAddress = auditLog.IpAddress,
            UserAgent = auditLog.UserAgent,
            Source = auditLog.Source,
            Timestamp = auditLog.Timestamp,
            AdditionalData = auditLog.AdditionalData,
            CreatedAt = auditLog.CreatedAt,
            UpdatedAt = auditLog.UpdatedAt
        };
    }
}

public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogByIdQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<AuditLogDto?> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
    {
        var auditId = AuditId.Create(request.AuditId);
        var auditLog = await _auditLogRepository.GetByIdAsync(auditId, cancellationToken);

        return auditLog != null ? AuditLogMapper.MapToDto(auditLog) : null;
    }
}

public class GetAuditLogsByUserQueryHandler : IRequestHandler<GetAuditLogsByUserQuery, IEnumerable<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogsByUserQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<IEnumerable<AuditLogDto>> Handle(GetAuditLogsByUserQuery request, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(request.UserId);
        var auditLogs = await _auditLogRepository.GetByUserIdAsync(userId, cancellationToken);

        return auditLogs.Select(AuditLogMapper.MapToDto);
    }
}

public class GetRecentAuditsQueryHandler : IRequestHandler<GetRecentAuditsQuery, IEnumerable<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetRecentAuditsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<IEnumerable<AuditLogDto>> Handle(GetRecentAuditsQuery request, CancellationToken cancellationToken)
    {
        var auditLogs = await _auditLogRepository.GetRecentAuditsAsync(request.Count, cancellationToken);

        return auditLogs.Select(AuditLogMapper.MapToDto);
    }
}
