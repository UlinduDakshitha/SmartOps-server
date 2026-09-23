using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SmartOps.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task JoinUserGroup(Guid userId)
    {
        var currentUserId = Context.User?.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(currentUserId, out var parsedUserId))
            throw new HubException("User identity is invalid.");

        if (parsedUserId != userId)
            throw new HubException(
                "You can only join your own notification group.");

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            GetUserGroupName(userId));
    }

    public async Task LeaveUserGroup(Guid userId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            GetUserGroupName(userId));
    }

    private static string GetUserGroupName(Guid userId)
        => $"user:{userId}";
}