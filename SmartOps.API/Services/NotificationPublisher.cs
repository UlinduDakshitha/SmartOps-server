using Microsoft.AspNetCore.SignalR;
using SmartOps.API.Hubs;
using SmartOps.Application.Interfaces;

namespace SmartOps.API.Services;

public class NotificationPublisher : INotificationPublisher
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationPublisher(
        IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PublishAsync(
        Guid userId,
        string title,
        string message,
        string type)
    {
        await _hubContext.Clients
            .Group($"user:{userId}")
            .SendAsync(
                "ReceiveNotification",
                new
                {
                    Title = title,
                    Message = message,
                    Type = type,
                    CreatedAt = DateTime.UtcNow
                });
    }
}