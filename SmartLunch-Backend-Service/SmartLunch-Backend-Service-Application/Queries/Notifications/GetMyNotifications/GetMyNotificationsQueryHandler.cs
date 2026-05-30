using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Notifications;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Notifications.GetMyNotifications;

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, GetMyNotificationsResponse>
{
    private readonly INotificationRepository _notifications;

    public GetMyNotificationsQueryHandler(INotificationRepository notifications) => _notifications = notifications;

    public async Task<GetMyNotificationsResponse> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _notifications.GetForUserAsync(
            request.UserId, request.Page, request.PageSize, request.UnreadOnly, cancellationToken);
        var unread = await _notifications.CountUnreadAsync(request.UserId, cancellationToken);

        return new GetMyNotificationsResponse
        {
            Data = items.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                Link = n.Link,
                SendAt = n.SendAt,
            }).ToList(),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize,
            UnreadCount = unread,
        };
    }
}
