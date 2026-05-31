using System.Text.RegularExpressions;

namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

public static class OrganizationChatbotIntents
{
    public const string Greeting = "greeting";
    public const string Orders = "orders";
    public const string OrderDetail = "order_detail";
    public const string OrderAmount = "order_amount";
    public const string OrderCount = "order_count";
    public const string Contract = "contract";
    public const string Menu = "menu";
    public const string Payment = "payment";
    public const string Delivery = "delivery";
    public const string Complaint = "complaint";
    public const string Cutoff = "cutoff";
    public const string PlaceOrder = "place_order";
    public const string Profile = "profile";
    public const string Contact = "contact";
    public const string FoodSafety = "food_safety";
    public const string Help = "help";
}

public static class OrganizationChatbotIntentClassifier
{
    private static readonly (string Intent, string[] Keywords, double Weight)[] Rules =
    [
        (OrganizationChatbotIntents.Greeting, ["xin chào", "chào bạn", "hello", "hey", "good morning", "good afternoon"], 1.0),
        (OrganizationChatbotIntents.OrderDetail, ["mã đơn", "invoice", "hd-", "dv-", "ord-"], 1.2),
        (OrganizationChatbotIntents.Orders, ["đơn hàng", "order", "suất", "đặt suất", "lịch giao", "danh sách đơn"], 1.0),
        (OrganizationChatbotIntents.Contract, ["hợp đồng", "contract", "ký hợp đồng", "thỏa thuận", "phụ lục"], 1.1),
        (OrganizationChatbotIntents.Menu, ["thực đơn", "menu", "món ăn", "tuần này", "tuần sau", "tuần tới", "hôm nay ăn"], 1.1),
        (OrganizationChatbotIntents.Payment, ["thanh toán", "payos", "cọc", "chưa trả", "unpaid", "còn nợ", "phải trả"], 1.2),
        (OrganizationChatbotIntents.Delivery, ["giao hàng", "shipper", "vận chuyển", "otp", "mã otp", "đang giao", "nhận hàng"], 1.1),
        (OrganizationChatbotIntents.Complaint, ["khiếu nại", "phản ánh", "complaint", "hoàn tiền", "thiếu suất", "sai món"], 1.1),
        (OrganizationChatbotIntents.Cutoff, ["chốt đơn", "deadline", "hạn chót", "khóa đơn", "17:00", "trước mấy giờ"], 1.2),
        (OrganizationChatbotIntents.Profile, ["hồ sơ", "đơn vị", "công ty", "tổ chức", "thông tin công ty", "profile"], 1.0),
        (OrganizationChatbotIntents.Contact, ["liên hệ", "hotline", "zalo", "gọi", "email", "tư vấn"], 1.0),
        (OrganizationChatbotIntents.FoodSafety, ["an toàn", "vệ sinh", "iso", "haccp", "truy xuất", "nguồn gốc"], 1.1),
    ];

    public static (string Intent, double Confidence) Classify(string message)
    {
        var normalized = Normalize(message);
        if (string.IsNullOrWhiteSpace(normalized))
            return (OrganizationChatbotIntents.Help, 0.2);

        if (LooksLikeOrderLookup(normalized))
            return (OrganizationChatbotIntents.OrderDetail, 0.95);

        if (IsAmountQuestion(normalized))
            return (OrganizationChatbotIntents.OrderAmount, 0.94);

        if (IsPaymentQuestion(normalized))
            return (OrganizationChatbotIntents.Payment, 0.93);

        if (IsCountQuestion(normalized))
            return (OrganizationChatbotIntents.OrderCount, 0.90);

        if (IsListOrdersQuestion(normalized))
            return (OrganizationChatbotIntents.Orders, 0.88);

        if (IsMenuQuestion(normalized))
            return (OrganizationChatbotIntents.Menu, 0.94);

        if (IsPlaceOrderQuestion(normalized))
            return (OrganizationChatbotIntents.PlaceOrder, 0.91);

        // Chào hỏi thật — không match "hi" trong "nhiêu"
        if (IsGreetingOnly(normalized))
            return (OrganizationChatbotIntents.Greeting, 0.85);

        var scores = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        foreach (var (intent, keywords, weight) in Rules)
        {
            foreach (var keyword in keywords)
            {
                if (ContainsKeyword(normalized, keyword))
                    scores[intent] = scores.GetValueOrDefault(intent) + weight;
            }
        }

        if (scores.Count == 0)
            return (OrganizationChatbotIntents.Help, 0.45);

        var best = scores.OrderByDescending(kv => kv.Value).First();
        var confidence = Math.Min(0.98, 0.55 + best.Value * 0.12);
        return (best.Key, confidence);
    }

    public static bool IsAmountQuestion(string normalized) =>
        AmountPatterns.Any(p => normalized.Contains(p, StringComparison.Ordinal));

    public static bool IsPaymentQuestion(string normalized) =>
        normalized.Contains("cần thanh toán") ||
        normalized.Contains("thanh toán bao nhiêu") ||
        normalized.Contains("phải thanh toán") ||
        (normalized.Contains("thanh toán") && normalized.Contains("bao nhiêu"));

    public static bool IsCountQuestion(string normalized) =>
        (normalized.Contains("bao nhiêu đơn") ||
         normalized.Contains("mấy đơn") ||
         normalized.Contains("số đơn") ||
         normalized.Contains("có bao nhiêu đơn")) &&
        !IsAmountQuestion(normalized) &&
        !IsPaymentQuestion(normalized);

    public static bool IsListOrdersQuestion(string normalized) =>
        normalized.Contains("danh sách") ||
        normalized.Contains("liệt kê") ||
        normalized.Contains("gần đây") ||
        (normalized.Contains("mới nhất") && normalized.Contains("đơn"));

    public static bool IsPlaceOrderQuestion(string normalized) =>
        normalized.Contains("đặt món") ||
        normalized.Contains("cách đặt") ||
        normalized.Contains("làm sao để đặt") ||
        normalized.Contains("hướng dẫn đặt") ||
        (normalized.Contains("đặt") && normalized.Contains("suất")) ||
        (normalized.Contains("làm sao") && normalized.Contains("đặt"));

    public static bool IsMenuQuestion(string normalized)
    {
        if (normalized.Contains("thực đơn") || normalized.Contains("menu"))
            return true;

        if (IsNextWeekMenuQuestion(normalized) || IsCurrentWeekMenuQuestion(normalized))
            return true;

        if (normalized.Contains("hôm nay ăn") || normalized.Contains("ăn gì"))
            return true;

        return normalized.Contains("món ăn") ||
               (normalized.Contains("món") && !IsPlaceOrderQuestion(normalized));
    }

    public static bool IsNextWeekMenuQuestion(string normalized) =>
        normalized.Contains("tuần sau") ||
        normalized.Contains("tuần tới") ||
        normalized.Contains("tuần kế");

    public static bool IsCurrentWeekMenuQuestion(string normalized) =>
        normalized.Contains("tuần này") ||
        normalized.Contains("tuần hiện tại");

    private static bool IsGreetingOnly(string normalized)
    {
        var stripped = normalized
            .Replace("ạ", "").Replace("nhé", "").Replace("!", "").Replace(".", "").Trim();
        return stripped is "xin chào" or "chào" or "hello" or "hi" or "hey"
               || stripped.StartsWith("xin chào ")
               || stripped.StartsWith("chào ");
    }

    private static readonly string[] AmountPatterns =
    [
        "tổng tiền", "tổng cộng", "tổng giá", "giá trị", "bao nhiêu tiền",
        "bn tiền", "còn nợ", "phải trả", "chưa trả", "nợ bao nhiêu",
        "số tiền", "thành tiền", "tổng đơn", "mấy tiền", "hết bao nhiêu",
        "cần thanh toán", "thanh toán bao nhiêu", "phải thanh toán"
    ];

    public static string? ExtractOrderSearchTerm(string message)
    {
        var invoice = Regex.Match(message, @"(?:HD|DV|ORD)[-\w]+", RegexOptions.IgnoreCase);
        if (invoice.Success)
            return invoice.Value;

        var idMatch = Regex.Match(message, @"\b(?:đơn|order|#)\s*(\d{1,8})\b", RegexOptions.IgnoreCase);
        if (idMatch.Success)
            return idMatch.Groups[1].Value;

        return null;
    }

    private static bool LooksLikeOrderLookup(string normalized)
    {
        if (Regex.IsMatch(normalized, @"(?:hd|dv|ord)[-\w]+", RegexOptions.IgnoreCase))
            return true;
        if (Regex.IsMatch(normalized, @"\b(?:đơn|order|#)\s*\d{1,8}\b", RegexOptions.IgnoreCase))
            return true;
        return false;
    }

    /// <summary>Tránh "hi" khớp trong "nhiêu", "nhiên"...</summary>
    private static bool ContainsKeyword(string normalized, string keyword)
    {
        if (keyword.Length <= 3)
        {
            var pattern = $@"\b{Regex.Escape(keyword)}\b";
            return Regex.IsMatch(normalized, pattern, RegexOptions.IgnoreCase);
        }
        return normalized.Contains(keyword, StringComparison.Ordinal);
    }

    public static string NormalizeForMatch(string message) => Normalize(message);

    private static string Normalize(string message) =>
        Regex.Replace(message.Trim().ToLowerInvariant(), @"\s+", " ");
}
