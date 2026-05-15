namespace SmartLunch.Backend.Service.Application.Helpers;

/// <summary>
/// Cửa sổ đặt suất theo nghiệp vụ: tối nay đặt thì <b>ngày mai là ngày thứ 1</b>, được chọn đúng 3 ngày liên tiếp (mai, mốt, kia).
/// Ví dụ tối thứ 4 → phục vụ thứ 5, 6, 7.
/// </summary>
public static class OrganizationMealOrderDateWindow
{
    /// <summary>Múi giờ Việt Nam (Windows + Linux).</summary>
    public static TimeZoneInfo VietnamTimeZone { get; } = ResolveVietnamTimeZone();

    public static DateOnly TodayInVietnam(DateTime utcNow) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc), VietnamTimeZone));

    /// <summary>Ngày phục vụ đầu tiên (mai) và cuối cùng (mai + 2).</summary>
    public static (DateOnly First, DateOnly Last) GetAllowedServiceDateRange(DateTime utcNow)
    {
        var today = TodayInVietnam(utcNow);
        var first = today.AddDays(1);
        var last = today.AddDays(3);
        return (first, last);
    }

    public static bool IsDateInWindow(DateOnly serviceDate, DateTime utcNow)
    {
        var (first, last) = GetAllowedServiceDateRange(utcNow);
        return serviceDate >= first && serviceDate <= last;
    }

    private static TimeZoneInfo ResolveVietnamTimeZone()
    {
        foreach (var id in new[] { "Asia/Ho_Chi_Minh", "SE Asia Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }

        return TimeZoneInfo.CreateCustomTimeZone(
            "UTC+07",
            TimeSpan.FromHours(7),
            "UTC+07",
            "UTC+07");
    }
}
