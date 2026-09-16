using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);

    Task<List<Team>> GetAllAsync();

    Task AddAsync(Team team);

    Task UpdateAsync(Team team);

    Task<bool> ExistsByNameAsync(string name);
}