using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface ITeamMemberRepository
{
    Task<bool> ExistsAsync(Guid teamId, Guid userId);

    Task AddAsync(TeamMember teamMember);

    Task RemoveAsync(Guid teamId, Guid userId);

    Task<List<User>> GetMembersAsync(Guid teamId);
}