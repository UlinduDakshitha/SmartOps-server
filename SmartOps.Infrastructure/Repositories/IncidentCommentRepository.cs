using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class IncidentCommentRepository : IIncidentCommentRepository
{
    private readonly SmartOpsDbContext _context;

    public IncidentCommentRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(IncidentComment comment)
    {
        await _context.IncidentComments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<List<IncidentComment>> GetByIncidentIdAsync(
        Guid incidentId)
    {
         return await _context.IncidentComments
             .Where(x => x.IncidentId == incidentId)
             .OrderBy(x => x.CreatedAt)
             .ToListAsync();
    }
}