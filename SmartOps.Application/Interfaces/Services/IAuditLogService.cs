using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Services;

public interface IAuditLogService
{
    Task LogAsync(
        Guid? userId,
        string action,
        string entityName,
        Guid? entityId,
        string? oldValue,
        string? newValue);

    Task<List<AuditLog>> GetAllAsync();

    Task<List<AuditLog>> GetByUserIdAsync(Guid userId);
}