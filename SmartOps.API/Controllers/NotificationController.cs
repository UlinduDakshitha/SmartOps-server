using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartOps.Application.Interfaces.Services;

namespace SmartOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("fixed")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(
        INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = GetCurrentUserId();

        var notifications =
            await _notificationService.GetByUserIdAsync(userId);

        return Ok(notifications);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = GetCurrentUserId();

        await _notificationService.MarkAsReadAsync(
            id,
            userId);

        return Ok(new
        {
            message = "Notification marked as read."
        });
    }

    [HttpPatch("{id:guid}/unread")]
    public async Task<IActionResult> MarkAsUnread(Guid id)
    {
        var userId = GetCurrentUserId();

        await _notificationService.MarkAsUnreadAsync(
            id,
            userId);

        return Ok(new
        {
            message = "Notification marked as unread."
        });
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