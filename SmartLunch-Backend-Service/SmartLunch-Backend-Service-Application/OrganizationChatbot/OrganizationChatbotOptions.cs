namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

/// <summary>Cấu hình chatbot RAG + LLM (Gemini / OpenAI-compatible).</summary>
public sealed class OrganizationChatbotOptions
{
    public const string SectionKey = "OrganizationChatbot";

    /// <summary>Gemini | OpenAI | None — None = chỉ rule-based fallback.</summary>
    public string Provider { get; set; } = "Gemini";

    public string? ApiKey { get; set; }

    /// <summary>Gemini: gemini-2.5-flash (khuyến nghị) hoặc gemini-2.0-flash.</summary>
    public string Model { get; set; } = "gemini-2.5-flash";

    /// <summary>OpenAI-compatible base URL (OpenAI provider only).</summary>
    public string? BaseUrl { get; set; }

    public bool Enabled { get; set; } = true;

    public double Temperature { get; set; } = 0.15;

    public int MaxOutputTokens { get; set; } = 1024;

    /// <summary>System prompt tùy chỉnh thay thế mặc định; null/empty = dùng mặc định.</summary>
    public string? SystemPrompt { get; set; }

    /// <summary>Quy tắc bổ sung admin — nối vào cuối system prompt.</summary>
    public string? CustomRules { get; set; }

    public bool IsConfigured =>
        Enabled &&
        !string.Equals(Provider, "None", StringComparison.OrdinalIgnoreCase) &&
        !string.IsNullOrWhiteSpace(ApiKey);

    public bool UsesDefaultSystemPrompt => string.IsNullOrWhiteSpace(SystemPrompt);

    public string ResolveCorePrompt() =>
        UsesDefaultSystemPrompt
            ? OrganizationChatbotDefaultPrompts.SystemPrompt
            : SystemPrompt!.Trim();

    public string ResolveSystemPrompt()
    {
        var core = ResolveCorePrompt();
        if (string.IsNullOrWhiteSpace(CustomRules))
            return core;
        return core + "\n\nQUY TẮC BỔ SUNG (admin):\n" + CustomRules.Trim();
    }
}
