using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface IIncidentCommentRepository
{
    Task AddAsync(IncidentComment comment);

    Task<List<IncidentComment>> GetByIncidentIdAsync(
        Guid incidentId);
}