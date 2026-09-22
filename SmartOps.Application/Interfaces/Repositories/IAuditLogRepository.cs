using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog);

    Task<List<AuditLog>> GetAllAsync();

    Task<List<AuditLog>> GetByUserIdAsync(Guid userId);
}