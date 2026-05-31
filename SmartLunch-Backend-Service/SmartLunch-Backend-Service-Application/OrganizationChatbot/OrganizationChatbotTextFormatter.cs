using System.Text.RegularExpressions;

namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

/// <summary>Làm sạch markdown thô từ LLM trước khi trả về client.</summary>
public static class OrganizationChatbotTextFormatter
{
    public static string CleanReply(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var t = text.Trim();

        // Bỏ block "Gợi ý:" — FE dùng chips riêng
        var suggestIdx = t.LastIndexOf("Gợi ý:", StringComparison.OrdinalIgnoreCase);
        if (suggestIdx >= 0)
            t = t[..suggestIdx].Trim();

        // **bold** → plain (FE sẽ format lại)
        t = Regex.Replace(t, @"\*\*([^*]+)\*\*", "$1");
        t = Regex.Replace(t, @"^\*\s+", "• ", RegexOptions.Multiline);
        t = t.Replace("**", string.Empty);

        // Giờ 17:00 → 17h00 (tránh lỗi hiển thị trên web)
        t = Regex.Replace(t, @"\b(\d{1,2}):(\d{2})\b", "$1h$2");

        // Gom nhiều dòng trống
        t = Regex.Replace(t, @"\n{3,}", "\n\n");

        return t.Trim();
    }
}
