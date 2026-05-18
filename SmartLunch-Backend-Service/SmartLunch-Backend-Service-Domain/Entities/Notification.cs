namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Notification entity mapped to SQL: notifications
/// </summary>
public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }

    /// <summary>
    /// Title of the notification (maps to Title in DB)
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification message content (maps to Message in DB)
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Notification type: info | warning | success | error (maps to Type in DB)
    /// </summary>
    public string Type { get; set; } = "info";

    /// <summary>
    /// Indicates if notification has been read (maps to IsRead in DB)
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// Link for notification redirection (maps to Link in DB, nullable)
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// When notification was sent (maps to SendAt in DB)
    /// </summary>
    public DateTime SendAt { get; set; } = VietnamTime.Now;

    public virtual User User { get; set; } = null!;
}
