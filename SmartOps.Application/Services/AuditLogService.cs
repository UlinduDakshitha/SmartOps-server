using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;

namespace SmartOps.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(
        IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task LogAsync(
        Guid? userId,
        string action,
        string entityName,
        Guid? entityId,
        string? oldValue,
        string? newValue)
    {
        var auditLog = new AuditLog(
            userId,
            action,
            entityName,
            entityId,
            oldValue,
            newValue);

        await _auditLogRepository.AddAsync(auditLog);
    }

    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _auditLogRepository.GetAllAsync();
    }

    public async Task<List<AuditLog>> GetByUserIdAsync(Guid userId)
    {
        return await _auditLogRepository.GetByUserIdAsync(userId);
    }
}