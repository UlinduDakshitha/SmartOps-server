using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOps.Application.DTOs.Incidents;
using SmartOps.Application.Interfaces.Services;

namespace SmartOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "IncidentHandler")]
public class IncidentController : ControllerBase
{
    private readonly IIncidentService _incidentService;

    public IncidentController(IIncidentService incidentService)
    {
        _incidentService = incidentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var incidents = await _incidentService.GetAllAsync();
        return Ok(incidents);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var incident = await _incidentService.GetByIdAsync(id);
        return Ok(incident);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateIncidentRequest request)
    {
        var userId = GetCurrentUserId();

        var incident = await _incidentService.CreateAsync(
            request,
            userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = incident.Id },
            incident);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateIncidentRequest request)
    {
        var incident = await _incidentService.UpdateAsync(
            id,
            request);

        return Ok(incident);
    }

    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> Assign(
        Guid id,
        AssignIncidentRequest request)
    {
        var userId = GetCurrentUserId();

        await _incidentService.AssignAsync(
            id,
            request,
            userId);

        return Ok(new
        {
            message = "Incident assigned successfully."
        });
    }

    [HttpPatch("{id:guid}/start")]
    public async Task<IActionResult> StartProgress(Guid id)
    {
        await _incidentService.StartProgressAsync(id);

        return Ok(new
        {
            message = "Incident moved to in-progress."
        });
    }

    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(
        Guid id,
        ResolveIncidentRequest request)
    {
        await _incidentService.ResolveAsync(
            id,
            request);

        return Ok(new
        {
            message = "Incident resolved successfully."
        });
    }

    [HttpPatch("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id)
    {
        await _incidentService.CloseAsync(id);

        return Ok(new
        {
            message = "Incident closed successfully."
        });
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _incidentService.CancelAsync(id);

        return Ok(new
        {
            message = "Incident cancelled successfully."
        });
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(
        Guid id,
        AddIncidentCommentRequest request)
    {
        var userId = GetCurrentUserId();

        var comment = await _incidentService.AddCommentAsync(
            id,
            request,
            userId);

        return Ok(comment);
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