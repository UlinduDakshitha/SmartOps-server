using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly SmartOpsDbContext _context;

    public RoleRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}