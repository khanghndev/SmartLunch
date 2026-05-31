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

    public double Temperature { get; set; } = 0.15;

    public int MaxOutputTokens { get; set; } = 1024;

    public bool Enabled =>
        !string.Equals(Provider, "None", StringComparison.OrdinalIgnoreCase) &&
        !string.IsNullOrWhiteSpace(ApiKey);
}
