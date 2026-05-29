namespace Khoa_Luan_KS_Web.Services;

/// <summary>
/// Lọc thực đơn tuần / lịch món cho khách hàng: ẩn tuần và ngày đã qua.
/// </summary>
public static class WeeklyMenuCustomerVisibility
{
    public static DateTime Today => DateTime.Today;

    public static IEnumerable<WeeklyMenuClientDto> FilterOrderableWeeks(IEnumerable<WeeklyMenuClientDto> menus)
    {
        var today = Today;
        return menus
            .Where(m => m.EndDate.Date >= today)
            .GroupBy(m => (m.StartDate.Date, m.EndDate.Date))
            .Select(g => g.OrderByDescending(x => x.Id).First())
            .OrderByDescending(m => m.StartDate);
    }

    public static WeeklyMenuClientDto? PickDefaultWeek(IEnumerable<WeeklyMenuClientDto> menus)
    {
        var list = FilterOrderableWeeks(menus).ToList();
        if (list.Count == 0)
            return null;

        var today = Today;
        return list.FirstOrDefault(m => m.StartDate.Date <= today && m.EndDate.Date >= today)
               ?? list.Where(m => m.StartDate.Date > today).OrderBy(m => m.StartDate).FirstOrDefault()
               ?? list.First();
    }

    public static List<WeeklyMenuScheduleDetailClientDto> FilterFutureSchedules(
        IEnumerable<WeeklyMenuScheduleDetailClientDto> schedules)
        => schedules.Where(s => s.Date.Date >= Today).ToList();

    public static void ApplyFutureSchedulesOnly(GetWeeklyMenuDetailClientResponse? detail)
    {
        if (detail == null)
            return;
        detail.Schedules = FilterFutureSchedules(detail.Schedules);
    }
}
