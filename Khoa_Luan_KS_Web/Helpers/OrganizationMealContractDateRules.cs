namespace Khoa_Luan_KS_Web.Helpers;

/// <summary>Quy tắc HĐ theo kỳ: bắt đầu từ tháng sau, tối đa ~1 tháng (62 ngày).</summary>
public static class OrganizationMealContractDateRules
{
    private static readonly TimeZoneInfo VietnamTz =
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

    public static DateOnly TodayVietnam()
    {
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTz);
        return DateOnly.FromDateTime(now.Date);
    }

    /// <summary>Ngày đầu tháng kế tiếp (mặc định bắt đầu HĐ).</summary>
    public static DateOnly GetDefaultPeriodStart(DateOnly today) =>
        new DateOnly(today.Year, today.Month, 1).AddMonths(1);

  public static DateOnly GetDefaultPeriodEnd(DateOnly start) =>
        start.AddMonths(1).AddDays(-1);

    public static DateOnly GetMinPeriodStart(DateOnly today) => GetDefaultPeriodStart(today);

    public static DateOnly GetMaxPeriodEnd(DateOnly start) => start.AddDays(61);

    public static string GetRuleHint(DateOnly today)
    {
        var start = GetDefaultPeriodStart(today);
        var end = GetDefaultPeriodEnd(start);
        return $"Hợp đồng theo kỳ bắt đầu từ {start:dd/MM/yyyy} (tháng sau). Thời hạn tối đa 1 tháng (đến {end:dd/MM/yyyy}).";
    }

    public static int CountServiceDays(DateOnly start, DateOnly end, IReadOnlyCollection<DateOnly> excluded)
    {
        if (end < start) return 0;
        var excludedInRange = excluded
            .Where(d => d >= start && d <= end)
            .Distinct()
            .Count();
        var inclusive = end.DayNumber - start.DayNumber + 1;
        return Math.Max(0, inclusive - excludedInRange);
    }

    public static decimal ComputeTotal(
        DateOnly start,
        DateOnly end,
        IReadOnlyCollection<DateOnly> excluded,
        int mealsPerDay,
        decimal mealUnitPrice)
    {
        var days = CountServiceDays(start, end, excluded);
        if (days < 1 || mealsPerDay < 1 || mealUnitPrice <= 0) return 0;
        return Math.Round(days * mealsPerDay * mealUnitPrice, 0, MidpointRounding.AwayFromZero);
    }

    public static DateOnly GetWeekMonday(DateOnly date)
    {
        var offset = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-offset);
    }

    public static IEnumerable<DateOnly> EnumerateServiceDates(
        DateOnly start,
        DateOnly end,
        IReadOnlyCollection<DateOnly> excluded)
    {
        var set = excluded.Where(d => d >= start && d <= end).ToHashSet();
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (!set.Contains(d))
                yield return d;
        }
    }

    public static IEnumerable<(DateOnly WeekMonday, DateOnly WeekEnd)> EnumerateWeeks(
        DateOnly contractStart,
        DateOnly contractEnd)
    {
        if (contractEnd < contractStart) yield break;
        var monday = GetWeekMonday(contractStart);
        while (monday <= contractEnd)
        {
            var weekEnd = monday.AddDays(6);
            yield return (monday, weekEnd > contractEnd ? contractEnd : weekEnd);
            monday = monday.AddDays(7);
        }
    }

    public static IEnumerable<DateOnly> GetWeekServiceDates(
        DateOnly weekMonday,
        DateOnly contractStart,
        DateOnly contractEnd,
        IReadOnlyCollection<DateOnly> excluded)
    {
        var weekEnd = weekMonday.AddDays(6);
        var rangeStart = weekMonday < contractStart ? contractStart : weekMonday;
        var rangeEnd = weekEnd > contractEnd ? contractEnd : weekEnd;
        if (rangeEnd < rangeStart) yield break;
        foreach (var d in EnumerateServiceDates(rangeStart, rangeEnd, excluded))
            yield return d;
    }
}
