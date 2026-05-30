using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.Commands.Notifications.MarkNotificationRead;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Response.Notifications;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Queries.Notifications.GetMyNotifications;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>Thông báo in-app (mọi role đã đăng nhập).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly INotificationRepository _notifications;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        IMediator mediator,
        INotificationRepository notifications,
        ILogger<NotificationController> logger)
    {
        _mediator = mediator;
        _notifications = notifications;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<BaseApiResponse<GetMyNotificationsResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? unreadOnly = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = RequireUserId();
            var response = await _mediator.Send(new GetMyNotificationsQuery(userId, page, pageSize, unreadOnly), cancellationToken);
            return Ok(BaseApiResponse<GetMyNotificationsResponse>.SuccessResult(response, "Notifications retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing notifications");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetMyNotificationsResponse>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<BaseApiResponse<UnreadCountResponse>>> UnreadCount(CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            var count = await _notifications.CountUnreadAsync(userId, cancellationToken);
            return Ok(BaseApiResponse<UnreadCountResponse>.SuccessResult(
                new UnreadCountResponse { UnreadCount = count }, "Unread count"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting unread notifications");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<UnreadCountResponse>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpPatch("{id:int}/read")]
    public async Task<ActionResult<BaseApiResponse<object>>> MarkRead(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            await _mediator.Send(new MarkNotificationReadCommand(userId, id), cancellationToken);
            return Ok(BaseApiResponse<object>.SuccessResult(new { }, "Marked as read"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<object>.NotFoundResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {Id} read", id);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    [HttpPost("read-all")]
    public async Task<ActionResult<BaseApiResponse<object>>> MarkAllRead(CancellationToken cancellationToken)
    {
        try
        {
            var userId = RequireUserId();
            await _mediator.Send(new MarkAllNotificationsReadCommand(userId), cancellationToken);
            return Ok(BaseApiResponse<object>.SuccessResult(new { }, "All marked as read"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications read");
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("Failed", new[] { ex.Message }));
        }
    }

    private int RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }
}
