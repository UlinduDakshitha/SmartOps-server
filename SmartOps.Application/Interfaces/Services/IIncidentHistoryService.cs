using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Services;

public interface IIncidentHistoryService
{
    Task<List<IncidentHistory>> GetByIncidentIdAsync(
        Guid incidentId);
}