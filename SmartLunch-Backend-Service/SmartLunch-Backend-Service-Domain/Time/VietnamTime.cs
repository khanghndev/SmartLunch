namespace SmartLunch.Backend.Service.Domain.Time;

/// <summary>
/// Giờ nghiệp vụ Việt Nam (UTC+7). Dùng cho ghi nhận thời điểm và so sánh ngày lịch trong hệ thống.
/// </summary>
public static class VietnamTime
{
    public static TimeZoneInfo TimeZone { get; } = ResolveVietnamTimeZone();

    /// <summary>Thời điểm hiện tại theo giờ Việt Nam.</summary>
    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);

    /// <summary>Ngày lịch hiện tại (giờ Việt Nam).</summary>
    public static DateOnly Today => DateOnly.FromDateTime(Now);

    /// <summary>00:00 của ngày lịch hiện tại.</summary>
    public static DateTime TodayAtMidnight =>
        DateTime.SpecifyKind(Today.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);

    /// <summary>
    /// Mốc 00:00 của một ngày phục vụ (lịch VN), lưu DB với Kind=Utc (quy ước dữ liệu hiện có).
    /// </summary>
    public static DateTime CalendarDateMidnight(DateOnly date) =>
        DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

    public static DateTime FromUtc(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(
            utc.Kind == DateTimeKind.Utc ? utc : DateTime.SpecifyKind(utc, DateTimeKind.Utc),
            TimeZone);

    public static (DateTime? Start, DateTime? EndExclusive) DayBounds(DateOnly? from, DateOnly? to)
    {
        if (!from.HasValue || !to.HasValue)
            return (null, null);

        return (CalendarDateMidnight(from.Value), CalendarDateMidnight(to.Value.AddDays(1)));
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
            "Giờ Việt Nam",
            "Giờ Việt Nam");
    }
}
