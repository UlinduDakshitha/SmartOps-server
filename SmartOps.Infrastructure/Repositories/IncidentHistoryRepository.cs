using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class IncidentHistoryRepository : IIncidentHistoryRepository
{
    private readonly SmartOpsDbContext _context;

    public IncidentHistoryRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(IncidentHistory history)
    {
        await _context.IncidentHistories.AddAsync(history);
        await _context.SaveChangesAsync();
    }

    public async Task<List<IncidentHistory>> GetByIncidentIdAsync(
        Guid incidentId)
    {
        return await _context.IncidentHistories
            .Where(x => x.IncidentId == incidentId)
            .ToListAsync();
    }
}