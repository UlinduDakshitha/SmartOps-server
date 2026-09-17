using Microsoft.EntityFrameworkCore;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Domain.Entities;
using SmartOps.Infrastructure.Data;

namespace SmartOps.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly SmartOpsDbContext _context;

    public NotificationRepository(SmartOpsDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }
}