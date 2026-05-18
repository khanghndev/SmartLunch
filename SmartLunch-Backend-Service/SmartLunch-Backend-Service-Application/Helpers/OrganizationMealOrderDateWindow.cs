using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Helpers;

/// <summary>
/// Cửa sổ đặt suất theo nghiệp vụ: tối nay đặt thì <b>ngày mai là ngày thứ 1</b>, được chọn đúng 3 ngày liên tiếp (mai, mốt, kia).
/// Ví dụ tối thứ 4 → phục vụ thứ 5, 6, 7.
/// </summary>
public static class OrganizationMealOrderDateWindow
{
    public static TimeZoneInfo VietnamTimeZone => VietnamTime.TimeZone;

    public static DateOnly TodayInVietnam() => VietnamTime.Today;

    public static DateOnly TodayInVietnam(DateTime referenceTime) =>
        DateOnly.FromDateTime(
            referenceTime.Kind == DateTimeKind.Utc
                ? VietnamTime.FromUtc(referenceTime)
                : referenceTime);

    /// <summary>Ngày phục vụ đầu tiên (mai) và cuối cùng (mai + 2).</summary>
    public static (DateOnly First, DateOnly Last) GetAllowedServiceDateRange()
    {
        var today = TodayInVietnam();
        return (today.AddDays(1), today.AddDays(3));
    }

    public static (DateOnly First, DateOnly Last) GetAllowedServiceDateRange(DateTime referenceTime)
    {
        var today = TodayInVietnam(referenceTime);
        return (today.AddDays(1), today.AddDays(3));
    }

    public static bool IsDateInWindow(DateOnly serviceDate) =>
        IsDateInWindow(serviceDate, VietnamTime.Now);

    public static bool IsDateInWindow(DateOnly serviceDate, DateTime referenceTime)
    {
        var (first, last) = GetAllowedServiceDateRange(referenceTime);
        return serviceDate >= first && serviceDate <= last;
    }
}
