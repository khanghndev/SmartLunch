namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

/// <summary>Mô tả quy tắc xử lý chatbot — hiển thị trên admin.</summary>
public static class OrganizationChatbotAdminCatalog
{
    public static IReadOnlyList<OrganizationChatbotRuleItem> GetProcessingRules() =>
    [
        new("Thực đơn / menu", "Câu hỏi về thực đơn tuần này/tuần sau → trả lời trực tiếp từ DB (T2–T7), không qua LLM.", true),
        new("RAG + Gemini", "Câu hỏi khác → lấy snapshot DB (đơn, HĐ, thanh toán, khiếu nại…) đưa vào Gemini.", true),
        new("Rule fallback", "Khi LLM lỗi hoặc tắt AI → trả lời theo template intent (payment, orders, cutoff…).", true),
        new("Không bịa dữ liệu", "Chỉ trả lời từ knowledge pack; thiếu dữ liệu thì nói rõ + hotline.", true),
        new("Plain text", "Không dùng Markdown; giờ chốt ghi dạng 17h00.", true),
    ];

    public static string ResolveLlmModeLabel(OrganizationChatbotOptions o)
    {
        if (!o.Enabled || string.Equals(o.Provider, "None", StringComparison.OrdinalIgnoreCase))
            return "Chỉ rule + DB (LLM tắt)";
        if (string.IsNullOrWhiteSpace(o.ApiKey))
            return "Chưa có API Key — fallback rule";
        return $"Gemini RAG ({o.Model})";
    }
}

public sealed class OrganizationChatbotRuleItem(string title, string description, bool isFixed)
{
    public string Title { get; } = title;
    public string Description { get; } = description;
    public bool IsFixed { get; } = isFixed;
}
