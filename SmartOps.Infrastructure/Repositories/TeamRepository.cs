using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly SmartOpsDbContext _context;

    public TeamRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Team>> GetAllAsync()
    {
        return await _context.Teams
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Team team)
    {
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Team team)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Teams
            .AnyAsync(x => x.Name == name);
    }
}