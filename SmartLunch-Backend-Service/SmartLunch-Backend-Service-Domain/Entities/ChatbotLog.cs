namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Chatbot conversation log
/// </summary>
public class ChatbotLog
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Response { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User? User { get; set; }
}
