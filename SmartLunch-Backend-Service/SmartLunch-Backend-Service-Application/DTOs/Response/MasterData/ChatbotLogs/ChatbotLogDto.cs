namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.ChatbotLogs;

public class ChatbotLogDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Response { get; set; }
    public DateTime CreatedAt { get; set; }
}
