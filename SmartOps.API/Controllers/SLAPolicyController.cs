using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOps.Application.DTOs.SLA;
using SmartOps.Application.Interfaces.Services;

namespace SmartOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class SLAPolicyController : ControllerBase
{
    private readonly ISLAPolicyService _slaPolicyService;

    public SLAPolicyController(
        ISLAPolicyService slaPolicyService)
    {
        _slaPolicyService = slaPolicyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var policies = await _slaPolicyService.GetAllAsync();

        return Ok(policies);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var policy = await _slaPolicyService.GetByIdAsync(id);

        return Ok(policy);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateSLAPolicyRequest request)
    {
        var policy = await _slaPolicyService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = policy.Id },
            policy);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateSLAPolicyRequest request)
    {
        var policy = await _slaPolicyService.UpdateAsync(
            id,
            request);

        return Ok(policy);
    }
}