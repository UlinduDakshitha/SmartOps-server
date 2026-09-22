using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOps.Application.Interfaces.Services;

namespace SmartOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogController(
        IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _auditLogService.GetAllAsync();

        return Ok(logs);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyLogs()
    {
        var userId = GetCurrentUserId();

        var logs = await _auditLogService
            .GetByUserIdAsync(userId);

        return Ok(logs);
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var parsedUserId))
            throw new UnauthorizedAccessException(
                "User identity is invalid.");

        return parsedUserId;
    }
}