using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Domain.Enums;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class IncidentRepository : IIncidentRepository
{
    private readonly SmartOpsDbContext _context;

    public IncidentRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task<Incident?> GetByIdAsync(Guid id)
    {
        return await _context.Incidents
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Incident?> GetByIncidentNumberAsync(
        string incidentNumber)
    {
        return await _context.Incidents
            .FirstOrDefaultAsync(
                x => x.IncidentNumber == incidentNumber);
    }

    public async Task<List<Incident>> GetAllAsync()
    {
        return await _context.Incidents
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Incident>> GetByStatusAsync(
        string status)
    {
        if (!Enum.TryParse<IncidentStatus>(
                status,
                true,
                out var incidentStatus))
        {
            return new List<Incident>();
        }

        return await _context.Incidents
            .Where(x => x.Status == incidentStatus)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Incident incident)
    {
        await _context.Incidents.AddAsync(incident);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Incident incident)
    {
        _context.Incidents.Update(incident);
        await _context.SaveChangesAsync();
    }
}