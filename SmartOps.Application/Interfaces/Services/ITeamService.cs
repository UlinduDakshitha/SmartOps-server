using SmartOps.Application.DTOs.Teams;
using SmartOps.Application.DTOs.Users;
namespace SmartOps.Application.Interfaces.Services;

public interface ITeamService
{
    Task<TeamResponse> GetByIdAsync(Guid id);

    Task<List<TeamResponse>> GetAllAsync();

    Task<TeamResponse> CreateAsync(
        CreateTeamRequest request);

    Task<TeamResponse> UpdateAsync(
        Guid id,
        UpdateTeamRequest request);

    Task AddMemberAsync(
        Guid teamId,
        AddTeamMemberRequest request);

    Task RemoveMemberAsync(
        Guid teamId,
        Guid userId);
    
    Task<List<UserResponse>> GetMembersAsync(Guid teamId);
    
    Task AssignTeamLeadAsync(
        Guid teamId,
        Guid userId);

    Task RemoveTeamLeadAsync(
        Guid teamId);
}