using System.Globalization;

namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

public static class OrganizationChatbotWeekHelper
{
    public static DateTime GetMonday(DateTime date)
    {
        var d = date.Date;
        var offset = ((int)d.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return d.AddDays(-offset);
    }

    /// <summary>Nhãn thứ theo web: T2 02/06/2026</summary>
    public static string FormatDayLabel(DateTime date)
    {
        var dow = date.DayOfWeek switch
        {
            DayOfWeek.Monday => "T2",
            DayOfWeek.Tuesday => "T3",
            DayOfWeek.Wednesday => "T4",
            DayOfWeek.Thursday => "T5",
            DayOfWeek.Friday => "T6",
            DayOfWeek.Saturday => "T7",
            DayOfWeek.Sunday => "CN",
            _ => date.DayOfWeek.ToString(),
        };
        return $"{dow} {date:dd/MM/yyyy}";
    }
}
