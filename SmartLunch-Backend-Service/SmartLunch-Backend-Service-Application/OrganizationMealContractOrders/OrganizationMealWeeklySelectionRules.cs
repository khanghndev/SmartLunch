namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

/// <summary>
/// Mỗi thời điểm chỉ mở đặt món cho một tuần (thường là tuần kế tiếp — ăn tuần sau thì chọn tuần này).
/// Không chọn trước thứ 6 → job tự random món theo số suất/ngày trong tuần đó.
/// </summary>
public static class OrganizationMealWeeklySelectionRules
{
    /// <summary>Tuần (Thứ 2) đang được phép chọn món; null nếu không còn tuần nào.</summary>
    public static DateOnly? ResolveOpenWeekMonday(
        DateOnly today,
        DateOnly contractStart,
        DateOnly contractEnd,
        IReadOnlyCollection<DateOnly> excludedDates)
    {
        var thisMonday = OrganizationMealPeriodContractCalculator.GetWeekMonday(today);
        var nextMonday = thisMonday.AddDays(7);

        if (contractStart < nextMonday && contractStart >= thisMonday)
        {
            var firstWeekMonday = OrganizationMealPeriodContractCalculator.GetWeekMonday(contractStart);
            if (WeekIsOpenForSelection(firstWeekMonday, today, contractStart, contractEnd, excludedDates))
                return firstWeekMonday;
        }

        if (WeekIsOpenForSelection(nextMonday, today, contractStart, contractEnd, excludedDates))
            return nextMonday;

        foreach (var (monday, _) in EnumerateContractWeeks(contractStart, contractEnd))
        {
            if (monday < thisMonday)
                continue;
            if (WeekIsOpenForSelection(monday, today, contractStart, contractEnd, excludedDates))
                return monday;
        }

        return null;
    }

    public static bool IsOpenWeekMonday(
        DateOnly weekMonday,
        DateOnly today,
        DateOnly contractStart,
        DateOnly contractEnd,
        IReadOnlyCollection<DateOnly> excludedDates)
    {
        var open = ResolveOpenWeekMonday(today, contractStart, contractEnd, excludedDates);
        return open.HasValue && open.Value == weekMonday;
    }

    public static bool WeekIsOpenForSelection(
        DateOnly weekMonday,
        DateOnly today,
        DateOnly contractStart,
        DateOnly contractEnd,
        IReadOnlyCollection<DateOnly> excludedDates)
    {
        var weekEnd = weekMonday.AddDays(6);
        if (weekEnd < today)
            return false;

        return OrganizationMealPeriodContractCalculator
            .GetWeekServiceDates(weekMonday, contractStart, contractEnd, excludedDates)
            .Any();
    }

    private static IEnumerable<(DateOnly WeekMonday, DateOnly WeekEnd)> EnumerateContractWeeks(
        DateOnly contractStart,
        DateOnly contractEnd)
    {
        if (contractEnd < contractStart)
            yield break;

        var monday = OrganizationMealPeriodContractCalculator.GetWeekMonday(contractStart);
        while (monday <= contractEnd)
        {
            var weekEnd = monday.AddDays(6);
            yield return (monday, weekEnd > contractEnd ? contractEnd : weekEnd);
            monday = monday.AddDays(7);
        }
    }
}
