using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(Guid id);

    Task<Incident?> GetByIncidentNumberAsync(
        string incidentNumber);

    Task<List<Incident>> GetAllAsync();

    Task<List<Incident>> GetByStatusAsync(
        string status);

    Task AddAsync(Incident incident);

    Task UpdateAsync(Incident incident);
}