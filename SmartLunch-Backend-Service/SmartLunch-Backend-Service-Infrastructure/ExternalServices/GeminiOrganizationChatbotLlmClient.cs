using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.Application.OrganizationChatbot;

namespace SmartLunch.Backend.Service.Infrastructure.ExternalServices;

/// <summary>
/// RAG + Gemini Flash — trả lời tự nhiên dựa trên knowledge pack từ DB.
/// Model khuyến nghị: gemini-2.0-flash (nhanh, tiếng Việt tốt, chi phí thấp).
/// </summary>
public sealed class GeminiOrganizationChatbotLlmClient : IOrganizationChatbotLlmClient
{
    private readonly HttpClient _http;
    private readonly OrganizationChatbotOptions _options;
    private readonly ILogger<GeminiOrganizationChatbotLlmClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };

    private const string SystemPrompt = """
        Bạn là trợ lý CSKH HuitMeal cho khách hàng doanh nghiệp (suất ăn công nghiệp).

        QUY TẮC BẮT BUỘC:
        1. CHỈ trả lời dựa trên JSON "accountData" được cung cấp — đây là dữ liệu thật từ hệ thống.
        2. KHÔNG bịa số tiền, mã đơn, ngày tháng, trạng thái. Nếu thiếu dữ liệu, nói rõ và gợi ý liên hệ hotline.
        3. Trả lời tiếng Việt, chuyên nghiệp, thân thiện, súc tích (tối đa 12 dòng).
        4. Khi hỏi số tiền / thanh toán / còn nợ:
           - Dùng summary.totalOutstandingVnd và pendingPaymentOrders nếu hỏi cần trả bao nhiêu.
           - Dùng orders[].totalAmountVnd nếu hỏi một đơn cụ thể hoặc đơn gần nhất.
           - Luôn ghi rõ số tiền VND có dấu phẩy ngàn (VD: 4.500.000 đ).
        5. Định dạng văn bản THUẦN — KHÔNG dùng Markdown:
           - KHÔNG dùng **, *, #, __ hoặc bất kỳ ký hiệu markdown nào.
           - Dùng bullet • cho danh sách.
           - Dùng dòng "Nhãn: Giá trị" (VD: Tiêu đề: Giao hàng trễ) — mỗi dòng chỉ một cặp nhãn:giá trị.
           - Giờ chốt đơn ghi dạng 17h00 (KHÔNG dùng dấu hai chấm trong giờ).
        6. Khi hỏi cách đặt món / làm sao đặt suất:
           - Dùng policies.howToOrder và cutoffRules — trả lời đủ 4 bước, không chỉ nêu hạn chốt.
           - Nếu có weeklyMenu thì nhắc chọn món trong thực đơn tuần.
        7. Không chào hỏi dài dòng nếu user đã hỏi câu cụ thể — trả lời thẳng vào câu hỏi trước.

        Cuối câu trả lời, thêm một dòng trống rồi dòng "Gợi ý:" với 3 câu hỏi ngắn (cách nhau bằng |).
        """;

    public GeminiOrganizationChatbotLlmClient(
        HttpClient http,
        IOptions<OrganizationChatbotOptions> options,
        ILogger<GeminiOrganizationChatbotLlmClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<LlmChatbotResult?> GenerateReplyAsync(
        string userMessage,
        OrganizationChatbotKnowledgePack knowledge,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled ||
            !string.Equals(_options.Provider, "Gemini", StringComparison.OrdinalIgnoreCase))
            return null;

        var model = string.IsNullOrWhiteSpace(_options.Model) ? "gemini-2.0-flash" : _options.Model;
        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(model)}:generateContent?key={Uri.EscapeDataString(_options.ApiKey!)}";

        var knowledgeJson = JsonSerializer.Serialize(knowledge, JsonOptions);
        var userPrompt = $"""
            accountData (JSON từ database):
            {knowledgeJson}

            Câu hỏi khách hàng: {userMessage}
            """;

        var payload = new
        {
            systemInstruction = new { parts = new[] { new { text = SystemPrompt } } },
            contents = new[]
            {
                new { role = "user", parts = new[] { new { text = userPrompt } } },
            },
            generationConfig = new
            {
                temperature = _options.Temperature,
                maxOutputTokens = _options.MaxOutputTokens,
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
