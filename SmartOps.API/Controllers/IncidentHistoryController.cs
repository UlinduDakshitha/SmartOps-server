using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartOps.Application.Interfaces.Services;

namespace SmartOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class IncidentHistoryController : ControllerBase
{
    private readonly IIncidentHistoryService _historyService;

    public IncidentHistoryController(
        IIncidentHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet("incident/{incidentId:guid}")]
    public async Task<IActionResult> GetByIncidentId(
        Guid incidentId)
    {
        var history = await _historyService
            .GetByIncidentIdAsync(incidentId);

        return Ok(history);
    }
}