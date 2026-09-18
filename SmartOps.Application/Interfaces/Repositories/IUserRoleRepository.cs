using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface IUserRoleRepository
{
    Task<List<Role>> GetRolesByUserIdAsync(Guid userId);

    Task AddAsync(UserRole userRole);

    Task<bool> ExistsAsync(Guid userId, Guid roleId);
}