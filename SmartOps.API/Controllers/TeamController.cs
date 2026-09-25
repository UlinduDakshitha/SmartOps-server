using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOps.Application.DTOs.Teams;
using SmartOps.Application.Interfaces.Services;

namespace SmartOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "TeamLeadOrAdmin")]
public class TeamController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teams = await _teamService.GetAllAsync();
        return Ok(teams);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var team = await _teamService.GetByIdAsync(id);
        return Ok(team);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamRequest request)
    {
        var team = await _teamService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTeamRequest request)
    {
        var team = await _teamService.UpdateAsync(id, request);
        return Ok(team);
    }

    [HttpPost("{teamId:guid}/members")]
    public async Task<IActionResult> AddMember(
        Guid teamId,
        AddTeamMemberRequest request)
    {
        await _teamService.AddMemberAsync(teamId, request);
        return Ok(new
        {
            message = "User added to team successfully."
        });
    }

    [HttpDelete("{teamId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid teamId,
        Guid userId)
    {
        await _teamService.RemoveMemberAsync(teamId, userId);
        return Ok(new
        {
            message = "User removed from team successfully."
        });
    }
    
    [HttpPatch("{teamId:guid}/lead/{userId:guid}")]
    public async Task<IActionResult> AssignTeamLead(
        Guid teamId,
        Guid userId)
    {
        await _teamService.AssignTeamLeadAsync(
            teamId,
            userId);

        return Ok(new
        {
            message = "Team lead assigned successfully."
        });
    }

    [HttpDelete("{teamId:guid}/lead")]
    public async Task<IActionResult> RemoveTeamLead(Guid teamId)
    {
        await _teamService.RemoveTeamLeadAsync(teamId);

        return Ok(new
        {
            message = "Team lead removed successfully."
        });
    }
    
    [HttpGet("{teamId:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid teamId)
    {
        var members = await _teamService.GetMembersAsync(teamId);

        return Ok(members);
    }
}