namespace SmartLunch.Backend.Service.Application.Helpers;

public static class FinanceContractSettlementQueryHelper
{
    public const int MaxDateRangeDays = 800;
    public const int MaxUnboundedOrders = 4000;

    public static void ValidateDatePair(DateOnly? from, DateOnly? to, string pairName)
    {
        if (from.HasValue ^ to.HasValue)
            throw new ArgumentException($"Cả hai giá trị From và To của '{pairName}' phải được gửi cùng nhau, hoặc cả hai đều bỏ trống.");
    }

    public static void AssertRangeDays(DateOnly from, DateOnly to)
    {
        if (to < from)
            throw new ArgumentException("Ngày kết thúc phải sau hoặc trùng ngày bắt đầu.");

        var days = to.DayNumber - from.DayNumber + 1;
        if (days > MaxDateRangeDays)
            throw new ArgumentException($"Khoảng ngày không được vượt quá {MaxDateRangeDays} ngày.");
    }

    public static (DateTime? StartUtc, DateTime? EndExclusiveUtc) ToUtcDayBounds(DateOnly? from, DateOnly? to)
    {
        if (!from.HasValue || !to.HasValue)
            return (null, null);

        var start = DateTime.SpecifyKind(from.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var endEx = DateTime.SpecifyKind(to.Value.ToDateTime(TimeOnly.MinValue).AddDays(1), DateTimeKind.Utc);
        return (start, endEx);
    }
}
