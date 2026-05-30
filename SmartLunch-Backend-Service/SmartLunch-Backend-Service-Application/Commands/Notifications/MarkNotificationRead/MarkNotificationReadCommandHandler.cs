using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.Notifications.MarkNotificationRead;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand>
{
    private readonly INotificationRepository _notifications;

    public MarkNotificationReadCommandHandler(INotificationRepository notifications) => _notifications = notifications;

    public Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken) =>
        _notifications.MarkReadAsync(request.NotificationId, request.UserId, cancellationToken);
}

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand>
{
    private readonly INotificationRepository _notifications;

    public MarkAllNotificationsReadCommandHandler(INotificationRepository notifications) => _notifications = notifications;

    public Task Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken) =>
        _notifications.MarkAllReadAsync(request.UserId, cancellationToken);
}
