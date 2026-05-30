using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface INotificationRepository
{
    Task CreateManyAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);
    Task<(List<Notification> Items, int TotalCount)> GetForUserAsync(
        int userId,
        int page,
        int pageSize,
        bool? unreadOnly = null,
        CancellationToken cancellationToken = default);
    Task<int> CountUnreadAsync(int userId, CancellationToken cancellationToken = default);
    Task<Notification?> GetByIdForUserAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task MarkReadAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(int userId, CancellationToken cancellationToken = default);
}
