using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly SmartLunchDBContext _context;

    public NotificationRepository(SmartLunchDBContext context) => _context = context;

    public async Task CreateManyAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default)
    {
        var list = notifications.ToList();
        if (list.Count == 0)
            return;

        await _context.Notifications.AddRangeAsync(list, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(List<Notification> Items, int TotalCount)> GetForUserAsync(
        int userId,
        int page,
        int pageSize,
        bool? unreadOnly = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId);
        if (unreadOnly == true)
            query = query.Where(n => !n.IsRead);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(n => n.SendAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<int> CountUnreadAsync(int userId, CancellationToken cancellationToken = default) =>
        _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

    public Task<Notification?> GetByIdForUserAsync(int id, int userId, CancellationToken cancellationToken = default) =>
        _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, cancellationToken);

    public async Task MarkReadAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdForUserAsync(id, userId, cancellationToken);
        if (entity == null)
            throw new KeyNotFoundException("Notification not found.");

        if (!entity.IsRead)
        {
            entity.IsRead = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAllReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), cancellationToken);
    }
}
