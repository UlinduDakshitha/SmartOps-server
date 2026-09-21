using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class TeamMemberRepository : ITeamMemberRepository
{
    private readonly SmartOpsDbContext _context;

    public TeamMemberRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid teamId, Guid userId)
    {
        return await _context.TeamMembers
            .AnyAsync(x => x.TeamId == teamId && x.UserId == userId);
    }

    public async Task AddAsync(TeamMember teamMember)
    {
        await _context.TeamMembers.AddAsync(teamMember);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid teamId, Guid userId)
    {
        var teamMember = await _context.TeamMembers
            .FirstOrDefaultAsync(x =>
                x.TeamId == teamId &&
                x.UserId == userId);

        if (teamMember is null)
            throw new KeyNotFoundException("Team member not found.");

        _context.TeamMembers.Remove(teamMember);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetMembersAsync(Guid teamId)
    {
        return await _context.TeamMembers
            .Where(x => x.TeamId == teamId)
            .Join(
                _context.Users,
                teamMember => teamMember.UserId,
                user => user.Id,
                (teamMember, user) => user)
            .ToListAsync();
    }
}