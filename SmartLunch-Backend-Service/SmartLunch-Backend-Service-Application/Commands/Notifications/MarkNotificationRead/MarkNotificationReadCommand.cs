using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.Notifications.MarkNotificationRead;

public record MarkNotificationReadCommand(int UserId, int NotificationId) : IRequest;

public record MarkAllNotificationsReadCommand(int UserId) : IRequest;
