using SmartOps.Domain.Entities;
using SmartOps.Domain.Enums;

namespace SmartOps.Application.Interfaces.Repositories;

public interface ISLAPolicyRepository
{
    Task<SLAPolicy?> GetByIdAsync(Guid id);

    Task<SLAPolicy?> GetByPriorityAsync(
        Priority priority);

    Task<List<SLAPolicy>> GetAllAsync();

    Task AddAsync(SLAPolicy policy);

    Task UpdateAsync(SLAPolicy policy);
}