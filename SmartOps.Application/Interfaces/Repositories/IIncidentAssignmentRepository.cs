using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface IIncidentAssignmentRepository
{
    Task AddAsync(IncidentAssignment assignment);

    Task<List<IncidentAssignment>> GetByIncidentIdAsync(
        Guid incidentId);

    Task<IncidentAssignment?> GetActiveByIncidentIdAsync(
        Guid incidentId);

    Task UpdateAsync(IncidentAssignment assignment);
}