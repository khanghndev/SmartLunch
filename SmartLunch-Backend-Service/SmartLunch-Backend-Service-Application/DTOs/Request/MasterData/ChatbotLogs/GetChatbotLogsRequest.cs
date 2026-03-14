namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.ChatbotLogs;

public class GetChatbotLogsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
