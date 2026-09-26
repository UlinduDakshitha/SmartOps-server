using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartOps.Application.Interfaces.Services;

namespace SmartOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ManagerOrAdmin")]
[EnableRateLimiting("fixed")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(
        IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _dashboardService.GetSummaryAsync();

        return Ok(summary);
    }
    [HttpGet("monthly-trend")]
    public async Task<IActionResult> GetMonthlyTrend()
    {
        var trend = await _dashboardService.GetMonthlyIncidentTrendAsync();
        return Ok(trend);
    }
    
    [HttpGet("team-workload")]
    public async Task<IActionResult> GetTeamWorkload()
    {
        var workload = await _dashboardService.GetTeamWorkloadAsync();
        return Ok(workload);
    }
}