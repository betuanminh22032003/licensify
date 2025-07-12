using MediatR;
using AuditService.Application.Commands.Audit;
using AuditService.Domain.Entities;
using AuditService.Domain.Repositories;
using AuditService.Domain.ValueObjects;

namespace AuditService.Application.Commands.Audit.Handlers;

public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, CreateAuditLogResponse>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAuditLogCommandHandler(
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork)
    {
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateAuditLogResponse> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId.HasValue ? UserId.Create(request.UserId.Value) : null;

        var auditLog = AuditLog.Create(
            userId,
            request.UserName,
            request.EntityType,
            request.EntityId,
            request.Action,
            request.Description,
            request.Severity,
            request.OldValues,
            request.NewValues,
            request.IpAddress,
            request.UserAgent,
            request.Source,
            request.AdditionalData);

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAuditLogResponse(
            auditLog.Id,
            auditLog.Timestamp
        );
    }
}
