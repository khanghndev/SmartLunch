using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Notifications;

namespace SmartLunch.Backend.Service.Application.Queries.Notifications.GetMyNotifications;

public record GetMyNotificationsQuery(int UserId, int Page = 1, int PageSize = 20, bool? UnreadOnly = null)
    : IRequest<GetMyNotificationsResponse>;
