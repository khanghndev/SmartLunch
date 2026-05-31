namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationChatbot;

public sealed class OrganizationChatbotAdminConfigDto
{
    public bool Enabled { get; set; }
    public string Provider { get; set; } = "Gemini";
    public string Model { get; set; } = "gemini-2.5-flash";
    public string? ApiKeyMasked { get; set; }
    public bool HasApiKey { get; set; }
    public double Temperature { get; set; }
    public int MaxOutputTokens { get; set; }

    /// <summary>Prompt lõi đang lưu (null = dùng mặc định).</summary>
    public string? StoredSystemPrompt { get; set; }
    public string DefaultSystemPrompt { get; set; } = string.Empty;
    public bool UseDefaultSystemPrompt { get; set; } = true;
    /// <summary>Prompt thực tế gửi Gemini ( lõi + quy tắc bổ sung).</summary>
    public string EffectiveSystemPrompt { get; set; } = string.Empty;
    public string? CustomRules { get; set; }

    public string Status { get; set; } = "offline";
    public string LlmModeLabel { get; set; } = string.Empty;
    public string ConfigSource { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public int? LastTestLatencyMs { get; set; }
    public string? LastTestMessage { get; set; }

    public List<OrganizationChatbotRuleItemDto> ProcessingRules { get; set; } = new();
}

public sealed class OrganizationChatbotRuleItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsFixed { get; set; }
}

public sealed class OrganizationChatbotTestResultDto
{
    public bool Success { get; set; }
    public int LatencyMs { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Model { get; set; }
}
