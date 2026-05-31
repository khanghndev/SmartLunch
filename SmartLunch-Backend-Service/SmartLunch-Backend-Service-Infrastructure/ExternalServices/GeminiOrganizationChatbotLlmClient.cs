using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.OrganizationChatbot;

namespace SmartLunch.Backend.Service.Infrastructure.ExternalServices;

/// <summary>RAG + Gemini Flash — trả lời tự nhiên dựa trên knowledge pack từ DB.</summary>
public sealed class GeminiOrganizationChatbotLlmClient : IOrganizationChatbotLlmClient
{
    private readonly HttpClient _http;
    private readonly IOrganizationChatbotSettingsProvider _settings;
    private readonly ILogger<GeminiOrganizationChatbotLlmClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };

    public GeminiOrganizationChatbotLlmClient(
        HttpClient http,
        IOrganizationChatbotSettingsProvider settings,
        ILogger<GeminiOrganizationChatbotLlmClient> logger)
    {
        _http = http;
        _settings = settings;
        _logger = logger;
    }

    public async Task<LlmChatbotResult?> GenerateReplyAsync(
        string userMessage,
        OrganizationChatbotKnowledgePack knowledge,
        CancellationToken cancellationToken = default)
    {
        var options = _settings.GetCurrent();
        if (!options.IsConfigured ||
            !string.Equals(options.Provider, "Gemini", StringComparison.OrdinalIgnoreCase))
            return null;

        var model = string.IsNullOrWhiteSpace(options.Model) ? "gemini-2.5-flash" : options.Model;
        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(model)}:generateContent?key={Uri.EscapeDataString(options.ApiKey!)}";

        var knowledgeJson = JsonSerializer.Serialize(knowledge, JsonOptions);
        var userPrompt = $"""
            accountData (JSON từ database):
            {knowledgeJson}

            Câu hỏi khách hàng: {userMessage}
            """;

        var payload = new
        {
            systemInstruction = new { parts = new[] { new { text = options.ResolveSystemPrompt() } } },
            contents = new[]
            {
                new { role = "user", parts = new[] { new { text = userPrompt } } },
            },
            generationConfig = new
            {
                temperature = options.Temperature,
                maxOutputTokens = options.MaxOutputTokens,
            },
        };

        try
        {
            using var res = await _http.PostAsJsonAsync(url, payload, JsonOptions, cancellationToken);
            var body = await res.Content.ReadAsStringAsync(cancellationToken);

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning("Gemini chatbot failed {Status}: {Body}", (int)res.StatusCode, Truncate(body));
                return null;
            }

            var parsed = JsonSerializer.Deserialize<GeminiResponse>(body, JsonOptions);
            var text = parsed?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return ParseReply(text, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini chatbot exception");
            return null;
        }
    }

    internal static LlmChatbotResult ParseReply(string fullText, string model)
    {
        var suggestions = new List<string>();
        var reply = fullText;

        var suggestIdx = fullText.LastIndexOf("Gợi ý:", StringComparison.OrdinalIgnoreCase);
        if (suggestIdx >= 0)
        {
            reply = fullText[..suggestIdx].Trim();
            var suggestPart = fullText[(suggestIdx + "Gợi ý:".Length)..].Trim();
            suggestions = suggestPart
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => s.Length > 2)
                .Take(4)
                .ToList();
        }

        if (suggestions.Count == 0)
        {
            suggestions = ["Tổng tiền cần thanh toán", "Đơn hàng gần đây", "Thực đơn tuần này"];
        }

        return new LlmChatbotResult
        {
            Reply = OrganizationChatbotTextFormatter.CleanReply(reply),
            Suggestions = suggestions,
            Model = model,
        };
    }

    private static string Truncate(string s) =>
        s.Length <= 400 ? s : s[..400] + "...";

    private sealed class GeminiResponse
    {
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    private sealed class GeminiCandidate
    {
        public GeminiContent? Content { get; set; }
    }

    private sealed class GeminiContent
    {
        public List<GeminiPart>? Parts { get; set; }
    }

    private sealed class GeminiPart
    {
        public string? Text { get; set; }
    }
}
