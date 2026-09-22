using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class IncidentAssignmentRepository : IIncidentAssignmentRepository
{
    private readonly SmartOpsDbContext _context;

    public IncidentAssignmentRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(IncidentAssignment assignment)
    {
        await _context.IncidentAssignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task<List<IncidentAssignment>> GetByIncidentIdAsync(
        Guid incidentId)
    {
        return await _context.IncidentAssignments
            .Where(x => x.IncidentId == incidentId)
            .OrderByDescending(x => x.AssignedAt)
            .ToListAsync();
    }

    public async Task<IncidentAssignment?> GetActiveByIncidentIdAsync(
        Guid incidentId)
    {
        return await _context.IncidentAssignments
            .FirstOrDefaultAsync(x =>
                x.IncidentId == incidentId &&
                x.UnassignedAt == null);
    }

    public async Task UpdateAsync(
        IncidentAssignment assignment)
    {
        _context.IncidentAssignments.Update(assignment);
        await _context.SaveChangesAsync();
    }
}