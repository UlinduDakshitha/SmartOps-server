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
    Task<List<Incident>> GetOpenIncidentsForSlaMonitoringAsync();
    
    Task<List<Incident>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? status = null,
        string? priority = null,
        string? severity = null);

    Task<int> GetCountAsync(
        string? status = null,
        string? priority = null,
        string? severity = null);

    Task AddAsync(Incident incident);
    
    Task AddWithOutboxAsync(
        Incident incident,
        AutomationOutbox outbox);

    Task UpdateAsync(Incident incident);

    Task UpdateWithOutboxAsync(
        Incident incident,
        AutomationOutbox outbox);
}