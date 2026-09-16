using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id);

    Task<List<Notification>> GetByUserIdAsync(
        Guid userId);

    Task AddAsync(Notification notification);

    Task UpdateAsync(Notification notification);
}