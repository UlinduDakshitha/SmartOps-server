using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOps.Application.DTOs.Users;
using SmartOps.Application.Interfaces.Services;

namespace SmartOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUserRequest request)
    {
        var result = await _userService.CreateAsync(request);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateUserRequest request)
    {
        var result = await _userService.UpdateAsync(
            id,
            request);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _userService.ActivateAsync(id);

        return Ok(new
        {
            Message = "User activated successfully."
        });
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _userService.DeactivateAsync(id);

        return Ok(new
        {
            Message = "User deactivated successfully."
        });
    }

    [HttpGet("{id:guid}/roles")]
    public async Task<IActionResult> GetRoles(Guid id)
    {
        var result = await _userService.GetRolesAsync(id);

        return Ok(result);
    }

    [HttpPost("{id:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> AssignRole(
        Guid id,
        Guid roleId)
    {
        await _userService.AssignRoleAsync(
            id,
            roleId);

        return Ok(new
        {
            Message = "Role assigned successfully."
        });
    }
}