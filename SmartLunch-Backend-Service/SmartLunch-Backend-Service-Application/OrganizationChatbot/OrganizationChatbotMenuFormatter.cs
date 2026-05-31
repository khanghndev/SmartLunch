using System.Globalization;
using System.Text;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

/// <summary>Trả lời thực đơn trực tiếp từ DB — không phụ thuộc LLM.</summary>
public static class OrganizationChatbotMenuFormatter
{
    public static bool IsNextWeekQuestion(string normalized) =>
        normalized.Contains("tuần sau") ||
        normalized.Contains("tuần tới") ||
        normalized.Contains("tuần kế") ||
        normalized.Contains("tuần tới nữa");

    public static bool IsCurrentWeekQuestion(string normalized) =>
        normalized.Contains("tuần này") ||
        normalized.Contains("tuần hiện tại");

    public static string FormatReply(string message, OrganizationChatbotKnowledgePack knowledge)
    {
        var normalized = OrganizationChatbotIntentClassifier.NormalizeForMatch(message);
        var wantNext = IsNextWeekQuestion(normalized);

        var menu = wantNext
            ? knowledge.WeeklyMenuNext ?? ProjectNextWeekFromCurrent(knowledge.WeeklyMenu)
            : knowledge.WeeklyMenu;

        if (menu?.Days == null || menu.Days.Count == 0)
        {
            if (wantNext)
                return "Chưa có thực đơn tuần sau trong hệ thống. Bạn có thể xem mục Thực đơn trên web khi bếp công bố, hoặc gọi hotline để được tư vấn.";

            return "Chưa có thực đơn tuần trong hệ thống.";
        }

        var periodLabel = wantNext ? "tuần sau" : IsCurrentWeekQuestion(normalized) ? "tuần này" : "tuần";
        var sb = new StringBuilder();
        sb.AppendLine($"Thực đơn {periodLabel} ({menu.WeekStart:dd/MM} – {menu.WeekEnd:dd/MM/yyyy})");

        if (menu.IsProjected)
            sb.AppendLine("(Dự kiến theo lịch món cố định theo thứ — tham khảo trên web khi bếp chốt tuần)");

        if (!string.IsNullOrWhiteSpace(menu.CustomerTypeName))
            sb.AppendLine($"Phân loại: {menu.CustomerTypeName}");

        sb.AppendLine();
        foreach (var day in menu.Days.OrderBy(d => d.Date))
        {
            var dishes = day.Dishes.Count > 0
                ? string.Join(", ", day.Dishes)
                : "Chưa có món";
            sb.AppendLine($"• {day.DayLabel}: {dishes}");
        }

        return sb.ToString().Trim();
    }

    public static List<string> DefaultSuggestions(OrganizationChatbotKnowledgePack k) =>
        k.WeeklyMenuNext?.Days.Count > 0
            ? ["Thực đơn tuần này", "Hạn chốt đơn", "Đơn hàng gần đây"]
            : ["Thực đơn tuần sau", "Hạn chốt đơn", "Đơn hàng gần đây"];

    public static WeeklyMenuKnowledge? MapWeeklyMenu(WeeklyMenu menu)
    {
        if (menu.MenuSchedules == null || menu.MenuSchedules.Count == 0)
            return null;

        return new WeeklyMenuKnowledge
        {
            WeekStart = menu.StartDate,
            WeekEnd = menu.EndDate,
            CustomerTypeName = menu.CustomerType?.Name,
            Days = menu.MenuSchedules
                .Where(s => s.Dish != null)
                .GroupBy(s => s.Date.Date)
                .OrderBy(g => g.Key)
                .Select(g => ToDailyKnowledge(g.Key, g.Select(s => s.Dish!.Name).Distinct().Take(8).ToList()))
                .ToList(),
        };
    }

    public static WeeklyMenuKnowledge? ProjectNextWeekFromCurrent(WeeklyMenuKnowledge? current)
    {
        if (current?.Days == null || current.Days.Count == 0)
            return null;

        var nextMonday = OrganizationChatbotWeekHelper.GetMonday(VietnamTime.Now).AddDays(7);
        var nextFriday = nextMonday.AddDays(4);

        var byDow = current.Days
            .GroupBy(d => d.Date.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.First().Dishes);

        var days = new List<DailyMenuKnowledge>();
        for (var d = nextMonday; d <= nextFriday; d = d.AddDays(1))
        {
            byDow.TryGetValue(d.DayOfWeek, out var dishes);
            days.Add(ToDailyKnowledge(d, dishes ?? new List<string>()));
        }

        return new WeeklyMenuKnowledge
        {
            WeekStart = nextMonday,
            WeekEnd = nextFriday,
            CustomerTypeName = current.CustomerTypeName,
            IsProjected = true,
            PeriodKey = "next",
            Days = days,
        };
    }

    private static DailyMenuKnowledge ToDailyKnowledge(DateTime date, List<string> dishes) =>
        new()
        {
            Date = date,
            DayLabel = OrganizationChatbotWeekHelper.FormatDayLabel(date),
            Dishes = dishes,
        };
}
