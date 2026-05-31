namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

/// <summary>Lịch phục vụ HĐ theo kỳ: ngày loại trừ + số suất tùy chỉnh theo ngày.</summary>
public static class OrganizationMealPeriodContractSchedule
{
    public static int ResolveMealsForDate(
        DateOnly date,
        int defaultMealsPerDay,
        IReadOnlyDictionary<DateOnly, int>? dailyOverrides)
    {
        if (defaultMealsPerDay < 1)
            throw new ArgumentException("Default meals per day must be at least 1.");
        if (dailyOverrides != null &&
            dailyOverrides.TryGetValue(date, out var custom) &&
            custom > 0)
        {
            return custom;
        }

        return defaultMealsPerDay;
    }

    public static Dictionary<DateOnly, int> NormalizeDailyOverrides(
        DateOnly startDate,
        DateOnly endDate,
        IReadOnlyCollection<DateOnly> excludedDates,
        int defaultMealsPerDay,
        IEnumerable<KeyValuePair<DateOnly, int>>? overrides)
    {
        var excluded = excludedDates
            .Where(d => d >= startDate && d <= endDate)
            .ToHashSet();

        var result = new Dictionary<DateOnly, int>();
        if (overrides == null)
            return result;

        foreach (var (date, count) in overrides)
        {
            if (date < startDate || date > endDate)
                throw new ArgumentException($"Daily meal override {date:yyyy-MM-dd} is outside contract period.");
            if (excluded.Contains(date))
                throw new ArgumentException($"Daily meal override {date:yyyy-MM-dd} is on an excluded date.");
            if (count < 1)
                throw new ArgumentException($"Meal count for {date:yyyy-MM-dd} must be at least 1.");
            if (count == defaultMealsPerDay)
                continue;
            result[date] = count;
        }

        return result;
    }

    public static Dictionary<DateOnly, int> ToOverrideDictionary(
        IEnumerable<ContractDailyMealPortionSource> portions) =>
        portions.ToDictionary(p => p.ServiceDate, p => p.MealCount);
}

public readonly record struct ContractDailyMealPortionSource(DateOnly ServiceDate, int MealCount);
