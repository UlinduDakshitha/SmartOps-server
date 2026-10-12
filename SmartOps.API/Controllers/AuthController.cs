
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartOps.Application.DTOs.Auth;
using SmartOps.Application.Interfaces.Services;

namespace SmartOps.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("fixed")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        var email = User.FindFirst(
            ClaimTypes.Email)?.Value;

        var roles = User.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToList();

        return Ok(new
        {
            UserId = userId,
            Email = email,
            Roles = roles
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromBody] string refreshToken)
    {
        var result = await _authService
            .RefreshTokenAsync(refreshToken);

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        [FromBody] string refreshToken)
    {
        await _authService
            .RevokeRefreshTokenAsync(refreshToken);

        return Ok(new
        {
            Message = "Logged out successfully."
        });
    }
}
