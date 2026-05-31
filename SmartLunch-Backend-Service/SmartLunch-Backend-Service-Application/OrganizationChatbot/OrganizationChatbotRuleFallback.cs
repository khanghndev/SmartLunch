using System.Globalization;
using System.Text;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

/// <summary>Fallback khi LLM không khả dụng — vẫn dùng knowledge pack từ DB.</summary>
public sealed class OrganizationChatbotRuleFallback
{
    public (string Reply, string Intent, double Confidence, List<string> Suggestions) Reply(
        string message,
        OrganizationChatbotKnowledgePack knowledge)
    {
        var normalized = OrganizationChatbotIntentClassifier.NormalizeForMatch(message);
        var (intent, confidence) = OrganizationChatbotIntentClassifier.Classify(message);

        var reply = intent switch
        {
            OrganizationChatbotIntents.Greeting => BuildGreeting(knowledge),
            OrganizationChatbotIntents.OrderAmount or OrganizationChatbotIntents.Payment
                => BuildPaymentAnswer(knowledge, normalized),
            OrganizationChatbotIntents.OrderCount => BuildCountAnswer(knowledge),
            OrganizationChatbotIntents.Orders => BuildOrdersList(knowledge),
            OrganizationChatbotIntents.OrderDetail => BuildOrderDetail(knowledge, message),
            OrganizationChatbotIntents.Contract => BuildContracts(knowledge),
            OrganizationChatbotIntents.Menu => BuildMenu(knowledge, message),
            OrganizationChatbotIntents.Delivery => BuildDelivery(knowledge),
            OrganizationChatbotIntents.Complaint => BuildComplaints(knowledge),
            OrganizationChatbotIntents.PlaceOrder => BuildPlaceOrder(knowledge),
            OrganizationChatbotIntents.Cutoff => BuildCutoff(knowledge),
            OrganizationChatbotIntents.Profile => BuildProfile(knowledge),
            OrganizationChatbotIntents.Contact => BuildContact(knowledge),
            OrganizationChatbotIntents.FoodSafety => BuildFoodSafety(knowledge),
            _ when IsPaymentLike(normalized) => BuildPaymentAnswer(knowledge, normalized),
            _ => BuildHelp(knowledge),
        };

        var suggestions = BuildSuggestions(intent, knowledge);
        return (reply, intent, confidence, suggestions);
    }

    public static List<string> DefaultSuggestions() =>
        ["Tổng tiền cần thanh toán", "Đơn hàng gần đây", "Thực đơn tuần này", "Hạn chốt đơn"];

    private static bool IsPaymentLike(string normalized) =>
        OrganizationChatbotIntentClassifier.IsAmountQuestion(normalized) ||
        normalized.Contains("thanh toán") ||
        normalized.Contains("cần trả") ||
        normalized.Contains("còn nợ");

    private static string BuildPaymentAnswer(OrganizationChatbotKnowledgePack k, string normalized)
    {
        var orgName = k.Organization?.Name ?? "đơn vị của bạn";

        if (k.PendingPaymentOrders.Count == 0)
        {
            var latest = k.Orders.FirstOrDefault();
            if (latest != null && !OrganizationChatbotLabels.IsAwaitingPayment(latest.PaymentStatus))
                return $"Hiện {orgName} không có khoản nào cần thanh toán.\n\nĐơn gần nhất ({latest.InvoiceCode ?? latest.Code}) trị giá {OrganizationChatbotLabels.FormatMoney(latest.TotalAmountVnd)} — {latest.PaymentStatusVi}.";
            if (latest != null)
                return FormatSingleOrderAmount(latest, orgName, k.Summary.TotalOrders);
            return "Chưa có đơn hàng trong hệ thống.";
        }

        // Câu hỏi kiểu "cần thanh toán bao nhiêu" → trả tổng nợ trước
        if (normalized.Contains("cần thanh toán") || normalized.Contains("còn nợ") ||
            normalized.Contains("phải trả") || normalized.Contains("chưa trả") ||
            normalized.Contains("thanh toán bao nhiêu") || normalized.Contains("mấy tiền"))
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{orgName} cần thanh toán:");
            sb.AppendLine();
            sb.AppendLine($"Tổng cộng: {OrganizationChatbotLabels.FormatMoney(k.Summary.TotalOutstandingVnd)}");
            sb.AppendLine($"Số đơn chờ thanh toán: {k.PendingPaymentOrders.Count}");
            sb.AppendLine();
            foreach (var o in k.PendingPaymentOrders.Take(5))
            {
                sb.AppendLine($"• {o.InvoiceCode ?? o.Code}");
                sb.AppendLine($"  {OrganizationChatbotLabels.FormatMoney(o.TotalAmountVnd)} — phục vụ {o.ScheduledDate:dd/MM/yyyy}");
                sb.AppendLine($"  {o.PaymentStatusVi}");
            }
            sb.AppendLine();
            sb.AppendLine("Thanh toán qua PayOS tại trang chi tiết đơn sau khi ký phụ lục.");
            return sb.ToString().Trim();
        }

        // "tổng tiền đơn hàng" → đơn gần nhất
        var recent = k.Orders.FirstOrDefault();
        return recent != null
            ? FormatSingleOrderAmount(recent, orgName, k.Summary.TotalOrders)
            : "Chưa có đơn hàng.";
    }

    private static string FormatSingleOrderAmount(OrderKnowledge o, string orgName, int totalOrders)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Đơn {o.InvoiceCode ?? o.Code}:");
        sb.AppendLine();
        sb.AppendLine($"Tổng tiền: {OrganizationChatbotLabels.FormatMoney(o.TotalAmountVnd)}");
        sb.AppendLine($"Ngày phục vụ: {o.ScheduledDate:dd/MM/yyyy}");
        sb.AppendLine($"Trạng thái: {o.StatusVi}");
        sb.AppendLine($"Thanh toán: {o.PaymentStatusVi}");
        if (totalOrders > 1)
        {
            sb.AppendLine();
            sb.AppendLine($"Bạn có {totalOrders} đơn. Hỏi \"cần thanh toán bao nhiêu\" để xem tổng nợ.");
        }
        return sb.ToString().Trim();
    }

    private static string BuildGreeting(OrganizationChatbotKnowledgePack k)
    {
        var orgName = k.Organization?.Name ?? "Quý đơn vị";
        var sb = new StringBuilder();
        sb.AppendLine($"Xin chào! Trợ lý HuitMeal — {orgName}.");
        sb.AppendLine();
        sb.AppendLine($"• Hợp đồng hiệu lực: {k.Summary.ActiveContracts}");
        sb.AppendLine($"• Tổng đơn hàng: {k.Summary.TotalOrders}");
        if (k.Summary.OrdersNeedingPayment > 0)
            sb.AppendLine($"• Cần thanh toán: {OrganizationChatbotLabels.FormatMoney(k.Summary.TotalOutstandingVnd)} ({k.Summary.OrdersNeedingPayment} đơn)");
        sb.AppendLine();
        sb.AppendLine("Hỏi trực tiếp: \"cần thanh toán bao nhiêu\", \"đơn gần đây\", \"thực đơn tuần này\".");
        return sb.ToString().Trim();
    }

    private static string BuildCountAnswer(OrganizationChatbotKnowledgePack k) =>
        $"Tổng số đơn: {k.Summary.TotalOrders}\n" +
        $"• Sắp tới / đang xử lý: {k.Summary.UpcomingOrders}\n" +
        $"• Cần thanh toán: {k.Summary.OrdersNeedingPayment}";

    private static string BuildOrdersList(OrganizationChatbotKnowledgePack k)
    {
        if (k.Orders.Count == 0)
            return "Chưa có đơn hàng. Đặt suất tại Dịch vụ → Thực đơn tự chọn hoặc Đặt suất theo hợp đồng.";

        var sb = new StringBuilder();
        sb.AppendLine($"{k.Summary.TotalOrders} đơn — {Math.Min(5, k.Orders.Count)} đơn gần nhất:");
        sb.AppendLine();
        foreach (var o in k.Orders.Take(5))
        {
            sb.AppendLine($"📦 {o.InvoiceCode ?? o.Code}");
            sb.AppendLine($"   {o.ScheduledDate:dd/MM/yyyy} · {o.StatusVi}");
            sb.AppendLine($"   {OrganizationChatbotLabels.FormatMoney(o.TotalAmountVnd)} · {o.PaymentStatusVi}");
            sb.AppendLine();
        }
        return sb.ToString().Trim();
    }

    private static string BuildOrderDetail(OrganizationChatbotKnowledgePack k, string message)
    {
        var search = OrganizationChatbotIntentClassifier.ExtractOrderSearchTerm(message);
        if (string.IsNullOrWhiteSpace(search))
            return BuildOrdersList(k);

        var match = k.Orders.FirstOrDefault(o =>
            (o.InvoiceCode?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
            o.Code.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            o.Id.ToString() == search);

        if (match == null)
            return $"Không tìm thấy đơn \"{search}\".";

        var sb = new StringBuilder();
        sb.AppendLine($"Đơn {match.InvoiceCode ?? match.Code}:");
        sb.AppendLine($"Tổng tiền: {OrganizationChatbotLabels.FormatMoney(match.TotalAmountVnd)}");
        sb.AppendLine($"Phục vụ: {match.ScheduledDate:dd/MM/yyyy} · {match.StatusVi}");
        sb.AppendLine($"Thanh toán: {match.PaymentStatusVi}");
        if (match.MainPortionCount > 0)
            sb.AppendLine($"Suất chính: {match.MainPortionCount}");
        if (match.DishNames.Count > 0)
            sb.AppendLine($"Món: {string.Join(", ", match.DishNames)}");
        return sb.ToString().Trim();
    }

    private static string BuildContracts(OrganizationChatbotKnowledgePack k)
    {
        if (k.Contracts.Count == 0)
            return "Chưa có hợp đồng đang hiệu lực.";

        var sb = new StringBuilder();
        sb.AppendLine($"{k.Contracts.Count} hợp đồng đang hiệu lực:");
        foreach (var c in k.Contracts.Take(3))
        {
            sb.AppendLine($"• {c.ContractNumber ?? c.Code}");
            if (c.MealUnitPriceVnd.HasValue)
                sb.AppendLine($"  Giá/suất: {OrganizationChatbotLabels.FormatMoney(c.MealUnitPriceVnd.Value)}");
            if (c.MealsPerDay.HasValue)
                sb.AppendLine($"  Suất/ngày: {c.MealsPerDay}");
        }
        return sb.ToString().Trim();
    }

    private static string BuildMenu(OrganizationChatbotKnowledgePack k, string message) =>
        OrganizationChatbotMenuFormatter.FormatReply(message, k);

    private static string BuildDelivery(OrganizationChatbotKnowledgePack k)
    {
        var active = k.Orders
            .Where(o => o.ScheduledDate.Date >= VietnamTime.Now.Date.AddDays(-1))
            .Take(5)
            .ToList();
        if (active.Count == 0)
            return "Không có đơn giao hàng trong vài ngày tới.";

        var sb = new StringBuilder();
        sb.AppendLine("Tình trạng giao hàng:");
        foreach (var o in active)
            sb.AppendLine($"• {o.InvoiceCode ?? o.Code} ({o.ScheduledDate:dd/MM}) — {o.StatusVi}" +
                          (o.DeliveryStatus != null ? $" · Giao: {o.DeliveryStatus}" : ""));
        return sb.ToString().Trim();
    }

    private static string BuildComplaints(OrganizationChatbotKnowledgePack k)
    {
        var sb = new StringBuilder();
        if (k.Complaints.Count > 0)
        {
            sb.AppendLine($"{k.Complaints.Count} khiếu nại gần đây:");
            foreach (var c in k.Complaints.Take(3))
                sb.AppendLine($"• {c.Title} — {c.StatusVi}");
            sb.AppendLine();
        }
        sb.AppendLine("Khiếu nại trong 24h sau khi đơn chuyển sang Đã giao.");
        return sb.ToString().Trim();
    }

    private static string BuildCutoff(OrganizationChatbotKnowledgePack k)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Quy định chốt đơn ({k.Organization?.Type ?? "Đơn vị"}):");
        sb.AppendLine($"• {k.CutoffRulesText}");
        var now = VietnamTime.Now;
        for (var i = 1; i <= 3; i++)
        {
            var d = now.Date.AddDays(i);
            var ok = CanOrder(k.Organization?.TypeKey ?? "office", d, now);
            sb.AppendLine($"• Phục vụ {d:dd/MM/yyyy}: {(ok ? "Còn đặt được" : "Đã qua hạn")}");
        }
        return sb.ToString().Trim();
    }

    private static string BuildPlaceOrder(OrganizationChatbotKnowledgePack k)
    {
        var sb = new StringBuilder();
        if (k.Policies.TryGetValue("howToOrder", out var steps) && !string.IsNullOrWhiteSpace(steps))
            sb.AppendLine(steps.Trim());
        else
            sb.AppendLine(OrganizationChatbotLabels.GetOrderHowToText(
                k.Organization?.TypeKey ?? "office",
                k.Organization?.Name));

        if (k.WeeklyMenu?.Days.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"Thực đơn tuần {k.WeeklyMenu.WeekStart:dd/MM} – {k.WeeklyMenu.WeekEnd:dd/MM} đang áp dụng — chọn món trong mục Thực đơn trên web.");
        }

        if (k.Contracts.Count == 0)
        {
            sb.AppendLine();
            sb.AppendLine("Lưu ý: Chưa thấy hợp đồng đang hiệu lực. Vui lòng liên hệ hotline để kích hoạt trước khi đặt món.");
        }

        return sb.ToString().Trim();
    }

    private static bool CanOrder(string orgType, DateTime serviceDate, DateTime now)
    {
        try
        {
            CutOffTimeValidator.Validate(orgType, serviceDate, now);
            return true;
        }
        catch { return false; }
    }

    private static string BuildProfile(OrganizationChatbotKnowledgePack k)
    {
        var o = k.Organization;
        if (o == null) return "Tài khoản chưa liên kết đơn vị.";
        var sb = new StringBuilder();
        sb.AppendLine($"Hồ sơ — {o.Name}:");
        sb.AppendLine($"• Loại: {o.Type}");
        if (!string.IsNullOrWhiteSpace(o.Address)) sb.AppendLine($"• Địa chỉ: {o.Address}");
        if (!string.IsNullOrWhiteSpace(o.Phone)) sb.AppendLine($"• ĐT: {o.Phone}");
        return sb.ToString().Trim();
    }

    private static string BuildContact(OrganizationChatbotKnowledgePack k)
    {
        var c = k.SupportContact;
        var sb = new StringBuilder();
        sb.AppendLine("Liên hệ hỗ trợ:");
        if (!string.IsNullOrWhiteSpace(c?.SupplierName)) sb.AppendLine($"• {c.SupplierName}");
        sb.AppendLine($"• Hotline: {c?.Hotline ?? "0334 297 551"}");
        sb.AppendLine($"• Email: {c?.Email ?? "contact@HuitMeal.com"}");
        return sb.ToString().Trim();
    }

    private static string BuildFoodSafety(OrganizationChatbotKnowledgePack k) =>
        "HuitMeal tuân thủ HACCP, ISO 22000 và truy xuất nguồn nguyên liệu qua hệ thống quản lý kho.";

    private static string BuildHelp(OrganizationChatbotKnowledgePack k)
    {
        var name = k.Organization?.Name ?? "đơn vị";
        return $"Tôi tra cứu dữ liệu thật của {name}.\n\n" +
               "Hỏi: \"cần thanh toán bao nhiêu\", \"đơn gần đây\", \"hợp đồng\", \"thực đơn tuần\", \"hạn chốt đơn\".";
    }

    private static List<string> BuildSuggestions(string intent, OrganizationChatbotKnowledgePack k)
    {
        if (k.Summary.OrdersNeedingPayment > 0)
            return ["Tổng tiền cần thanh toán", "Đơn hàng gần đây", "Hợp đồng hiện tại"];

        return intent switch
        {
            OrganizationChatbotIntents.OrderAmount or OrganizationChatbotIntents.Payment =>
                ["Đơn hàng gần đây", "Hợp đồng hiện tại", "Giao hàng"],
            OrganizationChatbotIntents.Menu =>
                ["Hạn chốt đơn", "Đơn hàng gần đây"],
            OrganizationChatbotIntents.PlaceOrder =>
                ["Thực đơn tuần này", "Hạn chốt đơn", "Đơn hàng gần đây"],
            _ => DefaultSuggestions(),
        };
    }
}
