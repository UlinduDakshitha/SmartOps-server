using SmartOps.Application.DTOs.Teams;
using SmartOps.Application.DTOs.Users;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;

namespace SmartOps.Application.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;

    private readonly ITeamMemberRepository _teamMemberRepository;

    public TeamService(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        ITeamMemberRepository teamMemberRepository)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<TeamResponse> GetByIdAsync(Guid id)
    {
        var team = await _teamRepository.GetByIdAsync(id);

        if (team is null)
        {
            throw new KeyNotFoundException(
                "Team not found.");
        }

        return MapToResponse(team);
    }

    public async Task<List<TeamResponse>> GetAllAsync()
    {
        var teams = await _teamRepository.GetAllAsync();

        return teams
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<TeamResponse> CreateAsync(
        CreateTeamRequest request)
    {
        var name = request.Name.Trim();

        if (await _teamRepository.ExistsByNameAsync(name))
        {
            throw new InvalidOperationException(
                "A team with this name already exists.");
        }

        var team = new Team(
            name,
            request.Description.Trim());

        await _teamRepository.AddAsync(team);

        return MapToResponse(team);
    }

    public async Task<TeamResponse> UpdateAsync(
        Guid id,
        UpdateTeamRequest request)
    {
        var team = await _teamRepository.GetByIdAsync(id);

        if (team is null)
        {
            throw new KeyNotFoundException(
                "Team not found.");
        }

        var name = request.Name.Trim();

        if (await _teamRepository.ExistsByNameAsync(name) &&
            !string.Equals(
                team.Name,
                name,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "A team with this name already exists.");
        }

        team.Update(
            name,
            request.Description.Trim());

        await _teamRepository.UpdateAsync(team);

        return MapToResponse(team);
    }

    public async Task AddMemberAsync(Guid teamId, AddTeamMemberRequest request)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team is null)
            throw new KeyNotFoundException("Team not found.");

        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user is null)
            throw new KeyNotFoundException("User not found.");

        if (await _teamMemberRepository.ExistsAsync(teamId, request.UserId))
            throw new InvalidOperationException("User is already a member of this team.");

        var teamMember = new TeamMember(teamId, request.UserId);

        await _teamMemberRepository.AddAsync(teamMember);
    }

    public async Task RemoveMemberAsync(Guid teamId, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team is null)
            throw new KeyNotFoundException("Team not found.");

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            throw new KeyNotFoundException("User not found.");
        
        if (team.TeamLeadId == userId)
        {
            throw new InvalidOperationException(
                "A team lead cannot be removed from the team. Remove the team lead first.");
        }

        await _teamMemberRepository.RemoveAsync(teamId, userId);
    }
    
    public async Task AssignTeamLeadAsync(
        Guid teamId,
        Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team is null)
        {
            throw new KeyNotFoundException(
                "Team not found.");
        }

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }
        if (!await _teamMemberRepository.ExistsAsync(teamId, userId))
        {
            throw new InvalidOperationException(
                "The user must be a member of the team before being assigned as team lead.");
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive user cannot be assigned as team lead.");
        }

        team.AssignTeamLead(userId);

        await _teamRepository.UpdateAsync(team);
    }

    public async Task RemoveTeamLeadAsync(Guid teamId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team is null)
        {
            throw new KeyNotFoundException(
                "Team not found.");
        }

        team.RemoveTeamLead();

        await _teamRepository.UpdateAsync(team);
    }
    private static TeamResponse MapToResponse(Team team)
    {
        return new TeamResponse
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            TeamLeadId = team.TeamLeadId,
            CreatedAt = team.CreatedAt,
            UpdatedAt = team.UpdatedAt,
             
        };
        
        
    }
    public async Task<List<UserResponse>> GetMembersAsync(Guid teamId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);

        if (team is null)
        {
            throw new KeyNotFoundException(
                "Team not found.");
        }

        var users = await _teamMemberRepository.GetMembersAsync(teamId);

        return users
            .Select(user => new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            })
            .ToList();
    }
}