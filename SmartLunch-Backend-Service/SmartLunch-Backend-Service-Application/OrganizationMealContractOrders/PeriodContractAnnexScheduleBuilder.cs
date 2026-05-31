using SmartLunch.Backend.Service.Application.Calendar;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

public sealed record PeriodContractDayRow(
    int Index,
    DateOnly Date,
    string WeekdayLabel,
    int MealCount,
    string? Note);

/// <summary>Lịch ngày phục vụ + số suất cho phụ lục HĐ Period-Based.</summary>
public static class PeriodContractAnnexScheduleBuilder
{
    public static bool IsPeriodContract(Contract? contract) =>
        contract != null &&
        string.Equals(contract.ContractType, OrganizationMealContractTypes.PeriodBased, StringComparison.OrdinalIgnoreCase);

    public static List<PeriodContractDayRow> BuildServiceDayRows(Contract contract)
    {
        if (!IsPeriodContract(contract))
            return new List<PeriodContractDayRow>();

        var start = DateOnly.FromDateTime(contract.StartDate);
        if (!contract.EndDate.HasValue)
            return new List<PeriodContractDayRow>();

        var end = DateOnly.FromDateTime(contract.EndDate.Value);
        var excluded = contract.ExcludedDates.Select(e => e.ExcludedDate).ToHashSet();
        var overrides = contract.DailyMealPortions.ToDictionary(p => p.ServiceDate, p => p.MealCount);
        var defaultMeals = contract.MealsPerDay is > 0 ? contract.MealsPerDay.Value : 1;

        var rows = new List<PeriodContractDayRow>();
        var index = 1;
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (excluded.Contains(d))
                continue;

            var meals = OrganizationMealPeriodContractSchedule.ResolveMealsForDate(d, defaultMeals, overrides);
            rows.Add(new PeriodContractDayRow(
                index++,
                d,
                ToWeekdayLabel(d),
                meals,
                VietnamesePublicHolidayCalendar.GetLabel(d)));
        }

        return rows;
    }

    public static List<(DateOnly Date, int Portions)> BuildDailyPortions(Contract contract) =>
        BuildServiceDayRows(contract).Select(r => (r.Date, r.MealCount)).ToList();

    public static int CountTotalMeals(Contract contract) =>
        BuildServiceDayRows(contract).Sum(r => r.MealCount);

    public static int CountExcludedDays(Contract contract)
    {
        if (!contract.EndDate.HasValue)
            return 0;
        var start = DateOnly.FromDateTime(contract.StartDate);
        var end = DateOnly.FromDateTime(contract.EndDate.Value);
        return contract.ExcludedDates.Count(d => d.ExcludedDate >= start && d.ExcludedDate <= end);
    }

    private static string ToWeekdayLabel(DateOnly date) =>
        date.DayOfWeek switch
        {
            DayOfWeek.Monday => "Thứ Hai",
            DayOfWeek.Tuesday => "Thứ Ba",
            DayOfWeek.Wednesday => "Thứ Tư",
            DayOfWeek.Thursday => "Thứ Năm",
            DayOfWeek.Friday => "Thứ Sáu",
            DayOfWeek.Saturday => "Thứ Bảy",
            DayOfWeek.Sunday => "Chủ nhật",
            _ => date.DayOfWeek.ToString(),
        };
}
