namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationChatbot;

public sealed class UpdateOrganizationChatbotConfigRequest
{
    public bool Enabled { get; set; } = true;
    public string Provider { get; set; } = "Gemini";
    /// <summary>Để trống hoặc gửi masked placeholder để giữ key cũ.</summary>
    public string? ApiKey { get; set; }
    public string Model { get; set; } = "gemini-2.5-flash";
    public double Temperature { get; set; } = 0.15;
    public int MaxOutputTokens { get; set; } = 1024;
    /// <summary>Null/empty = dùng prompt mặc định hệ thống.</summary>
    public string? SystemPrompt { get; set; }
    public string? CustomRules { get; set; }
    /// <summary>True = ghi đè bằng SystemPrompt; false = dùng default + CustomRules.</summary>
    public bool UseDefaultSystemPrompt { get; set; } = true;
}
