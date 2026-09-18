using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly SmartOpsDbContext _context;

    public UserRoleRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetRolesByUserIdAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(x => x.UserId == userId)
            .Join(
                _context.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => role)
            .ToListAsync();
    }

    public async Task AddAsync(UserRole userRole)
    {
        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(
        Guid userId,
        Guid roleId)
    {
        return await _context.UserRoles
            .AnyAsync(x =>
                x.UserId == userId &&
                x.RoleId == roleId);
    }
}