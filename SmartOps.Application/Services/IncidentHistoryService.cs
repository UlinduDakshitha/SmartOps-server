using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;

namespace SmartOps.Application.Services;

public class IncidentHistoryService : IIncidentHistoryService
{
    private readonly IIncidentHistoryRepository _historyRepository;
    private readonly IIncidentRepository _incidentRepository;

    public IncidentHistoryService(
        IIncidentHistoryRepository historyRepository,
        IIncidentRepository incidentRepository)
    {
        _historyRepository = historyRepository;
        _incidentRepository = incidentRepository;
    }

    public async Task<List<IncidentHistory>> GetByIncidentIdAsync(
        Guid incidentId)
    {
        var incident = await _incidentRepository.GetByIdAsync(incidentId);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        return await _historyRepository
            .GetByIncidentIdAsync(incidentId);
    }
}