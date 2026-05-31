namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

/// <summary>Quy tắc HĐ theo kỳ: bắt đầu tối thiểu sau 3 ngày, tối đa 1 tháng.</summary>
public static class OrganizationMealPeriodContractDateRules
{
    public const int MinStartLeadDays = 3;

    public static DateOnly TodayVietnam() => VietnamTime.Today;

    public static DateOnly GetMinPeriodStart(DateOnly today) => today.AddDays(MinStartLeadDays);

    public static DateOnly GetDefaultPeriodStart(DateOnly today) => GetMinPeriodStart(today);

    public static DateOnly GetDefaultPeriodEnd(DateOnly start) => GetMaxPeriodEnd(start);

    public static DateOnly GetMaxPeriodEnd(DateOnly start) => start.AddMonths(1).AddDays(-1);

    public static void ValidatePeriod(DateOnly startDate, DateOnly endDate, DateOnly? today = null)
    {
        var todayValue = today ?? TodayVietnam();
        var minStart = GetMinPeriodStart(todayValue);
        if (startDate < minStart)
        {
            throw new ArgumentException(
                $"Start date must be at least {MinStartLeadDays} days from today (from {minStart:dd/MM/yyyy}).");
        }

        if (endDate < startDate)
            throw new ArgumentException("EndDate must be on or after StartDate.");

        var maxEnd = GetMaxPeriodEnd(startDate);
        if (endDate > maxEnd)
        {
            throw new ArgumentException(
                $"Contract period must not exceed one month (latest end date {maxEnd:dd/MM/yyyy}).");
        }
    }
}
