using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface IIncidentHistoryRepository
{
    Task AddAsync(IncidentHistory history);

    Task<List<IncidentHistory>> GetByIncidentIdAsync(
        Guid incidentId);
}