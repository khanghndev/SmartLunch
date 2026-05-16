namespace Khoa_Luan_KS_Web.Helpers;

/// <summary>
/// Quy tắc chọn ngày phục vụ: đặt trước tối thiểu 3 ngày; mặc định Thứ 2 tuần sau;
/// nếu hôm nay là Thứ 7 thì sớm nhất là Thứ 3 tuần sau.
/// </summary>
public static class OrganizationMealOrderDateRules
{
    public static DateOnly TodayVietnam()
    {
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");
            return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz));
        }
        catch
        {
            return DateOnly.FromDateTime(DateTime.Now);
        }
    }

    /// <summary>Thứ Hai đầu tuần lịch kế tiếp (không tính tuần hiện tại).</summary>
    public static DateOnly GetMondayOfNextCalendarWeek(DateOnly today)
    {
        var daysFromMonday = today.DayOfWeek == DayOfWeek.Sunday
            ? 6
            : (int)today.DayOfWeek - (int)DayOfWeek.Monday;
        var mondayThisWeek = today.AddDays(-daysFromMonday);
        return mondayThisWeek.AddDays(7);
    }

    public static DateOnly GetMinimumServiceDate(DateOnly today)
    {
        var threeDaysAhead = today.AddDays(3);
        var nextMonday = GetMondayOfNextCalendarWeek(today);
        var min = threeDaysAhead > nextMonday ? threeDaysAhead : nextMonday;

        if (today.DayOfWeek == DayOfWeek.Saturday)
        {
            var tuesdayNextWeek = nextMonday.AddDays(1);
            if (min < tuesdayNextWeek)
                min = tuesdayNextWeek;
        }

        return min;
    }

    public static DateOnly GetDefaultServiceDate(DateOnly today)
    {
        var min = GetMinimumServiceDate(today);
        var preferred = GetMondayOfNextCalendarWeek(today);
        if (today.DayOfWeek == DayOfWeek.Saturday)
            preferred = preferred.AddDays(1);
        return preferred >= min ? preferred : min;
    }

    public static DateOnly GetMaximumServiceDate(DateOnly today, int horizonDays = 90) =>
        GetMinimumServiceDate(today).AddDays(horizonDays);

    public static bool IsAllowedServiceDate(DateOnly serviceDate, DateOnly today)
    {
        return serviceDate >= GetMinimumServiceDate(today)
               && serviceDate <= GetMaximumServiceDate(today);
    }

    public static string GetRuleHint(DateOnly today)
    {
        var min = GetMinimumServiceDate(today);
        if (today.DayOfWeek == DayOfWeek.Saturday)
        {
            return $"Đặt trước tối thiểu 3 ngày. Hôm nay là Thứ 7 — ngày phục vụ sớm nhất: {min:dd/MM/yyyy} (Thứ 3 tuần sau trở đi).";
        }

        return $"Đặt trước tối thiểu 3 ngày. Ngày phục vụ sớm nhất: {min:dd/MM/yyyy} (Thứ 2 tuần sau hoặc sau đó).";
    }
}
