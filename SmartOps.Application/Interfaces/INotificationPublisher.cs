namespace SmartOps.Application.Interfaces;

public interface INotificationPublisher
{
    Task PublishAsync(
        Guid userId,
        string title,
        string message,
        string type);
}