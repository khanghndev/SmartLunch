namespace SmartLunch.Backend.Service.Application.DTOs.Response.Notifications;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string? Link { get; set; }
    public DateTime SendAt { get; set; }
}

public class GetMyNotificationsResponse
{
    public List<NotificationDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int UnreadCount { get; set; }
}

public class UnreadCountResponse
{
    public int UnreadCount { get; set; }
}
