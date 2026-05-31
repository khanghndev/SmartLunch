namespace Khoa_Luan_KS_Web.Helpers;

/// <summary>Quy tắc HĐ theo kỳ: bắt đầu tối thiểu sau 3 ngày, tối đa 1 tháng.</summary>
public static class OrganizationMealContractDateRules
{
    public const int MinStartLeadDays = 3;

    private static readonly TimeZoneInfo VietnamTz =
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

    public static DateOnly TodayVietnam()
    {
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTz);
        return DateOnly.FromDateTime(now.Date);
    }

    /// <summary>Ngày bắt đầu sớm nhất (hôm nay + 3 ngày chuẩn bị).</summary>
    public static DateOnly GetMinPeriodStart(DateOnly today) => today.AddDays(MinStartLeadDays);

    public static DateOnly GetDefaultPeriodStart(DateOnly today) => GetMinPeriodStart(today);

    public static DateOnly GetDefaultPeriodEnd(DateOnly start) => GetMaxPeriodEnd(start);

    /// <summary>Ngày kết thúc muộn nhất: đúng 1 tháng kể từ ngày bắt đầu.</summary>
    public static DateOnly GetMaxPeriodEnd(DateOnly start) => start.AddMonths(1).AddDays(-1);

    public static string GetRuleHint(DateOnly today)
    {
        var minStart = GetMinPeriodStart(today);
        var sampleEnd = GetMaxPeriodEnd(minStart);
        return $"Ngày bắt đầu phải từ {minStart:dd/MM/yyyy} trở đi (đặt trước ít nhất {MinStartLeadDays} ngày để chuẩn bị suất ăn). " +
               $"Thời hạn tối đa 1 tháng (vd. bắt đầu {minStart:dd/MM/yyyy} → kết thúc tối đa {sampleEnd:dd/MM/yyyy}).";
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

    public static int CountTotalMeals(
        DateOnly start,
        DateOnly end,
        IReadOnlyCollection<DateOnly> excluded,
        int defaultMealsPerDay,
        IReadOnlyDictionary<string, int>? dailyOverridesByIso = null)
    {
        if (defaultMealsPerDay < 1) return 0;
        var total = 0;
        foreach (var date in EnumerateServiceDates(start, end, excluded))
        {
            var iso = date.ToString("yyyy-MM-dd");
            if (dailyOverridesByIso != null &&
                dailyOverridesByIso.TryGetValue(iso, out var custom) &&
                custom > 0)
            {
                total += custom;
            }
            else
            {
                total += defaultMealsPerDay;
            }
        }

        return total;
    }

    public static decimal ComputeTotal(
        DateOnly start,
        DateOnly end,
        IReadOnlyCollection<DateOnly> excluded,
        int mealsPerDay,
        decimal mealUnitPrice,
        IReadOnlyDictionary<string, int>? dailyOverridesByIso = null)
    {
        var totalMeals = CountTotalMeals(start, end, excluded, mealsPerDay, dailyOverridesByIso);
        if (totalMeals < 1 || mealUnitPrice <= 0) return 0;
        return Math.Round(totalMeals * mealUnitPrice, 0, MidpointRounding.AwayFromZero);
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
