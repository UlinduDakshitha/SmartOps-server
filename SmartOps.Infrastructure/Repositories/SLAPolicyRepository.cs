using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Domain.Enums;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class SLAPolicyRepository : ISLAPolicyRepository
{
    private readonly SmartOpsDbContext _context;

    public SLAPolicyRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task<SLAPolicy?> GetByIdAsync(Guid id)
    {
        return await _context.SLAPolicies
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<SLAPolicy?> GetByPriorityAsync(
        Priority priority)
    {
        return await _context.SLAPolicies
            .FirstOrDefaultAsync(x => x.Priority == priority);
    }

    public async Task<List<SLAPolicy>> GetAllAsync()
    {
        return await _context.SLAPolicies
            .OrderBy(x => x.Priority)
            .ToListAsync();
    }

    public async Task AddAsync(SLAPolicy policy)
    {
        await _context.SLAPolicies.AddAsync(policy);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SLAPolicy policy)
    {
        _context.SLAPolicies.Update(policy);
        await _context.SaveChangesAsync();
    }
}