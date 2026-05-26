namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

/// <summary>
/// Tổng HĐ theo kỳ = (Kết thúc − Bắt đầu − số ngày loại trừ + 1) × Số suất/ngày × Giá/suất.
/// </summary>
public static class OrganizationMealPeriodContractCalculator
{
    public static int CountServiceDays(DateOnly startDate, DateOnly endDate, IReadOnlyCollection<DateOnly> excludedDates)
    {
        if (endDate < startDate)
            throw new ArgumentException("EndDate must be on or after StartDate.");

        var excluded = excludedDates
            .Where(d => d >= startDate && d <= endDate)
            .Distinct()
            .Count();

        var inclusiveCalendarDays = endDate.DayNumber - startDate.DayNumber + 1;
        var serviceDays = inclusiveCalendarDays - excluded;
        if (serviceDays < 1)
            throw new ArgumentException("Contract must have at least one service day after exclusions.");

        return serviceDays;
    }

    public static decimal ComputeTotalValue(
        DateOnly startDate,
        DateOnly endDate,
        IReadOnlyCollection<DateOnly> excludedDates,
        int mealsPerDay,
        decimal mealUnitPrice)
    {
        if (mealsPerDay < 1)
            throw new ArgumentException("MealsPerDay must be at least 1.");
        if (mealUnitPrice <= 0)
            throw new ArgumentException("MealUnitPrice must be greater than zero.");

        var serviceDays = CountServiceDays(startDate, endDate, excludedDates);
        var total = serviceDays * mealsPerDay * mealUnitPrice;
        return decimal.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    public static IEnumerable<DateOnly> EnumerateServiceDates(
        DateOnly startDate,
        DateOnly endDate,
        IReadOnlyCollection<DateOnly> excludedDates)
    {
        var excludedSet = excludedDates
            .Where(d => d >= startDate && d <= endDate)
            .ToHashSet();

        for (var d = startDate; d <= endDate; d = d.AddDays(1))
        {
            if (!excludedSet.Contains(d))
                yield return d;
        }
    }

    /// <summary>Thứ 2 của tuần chứa <paramref name="date"/>.</summary>
    public static DateOnly GetWeekMonday(DateOnly date)
    {
        var offset = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-offset);
    }

    public static IEnumerable<DateOnly> GetWeekServiceDates(
        DateOnly weekMonday,
        DateOnly contractStart,
        DateOnly contractEnd,
        IReadOnlyCollection<DateOnly> excludedDates)
    {
        var weekEnd = weekMonday.AddDays(6);
        var rangeStart = weekMonday < contractStart ? contractStart : weekMonday;
        var rangeEnd = weekEnd > contractEnd ? contractEnd : weekEnd;
        if (rangeEnd < rangeStart)
            yield break;

        foreach (var d in EnumerateServiceDates(rangeStart, rangeEnd, excludedDates))
            yield return d;
    }
}
