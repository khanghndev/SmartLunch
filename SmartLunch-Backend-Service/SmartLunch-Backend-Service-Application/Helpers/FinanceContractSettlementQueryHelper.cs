using SmartLunch.Backend.Service.Domain.Time;

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

    /// <summary>Biên ngày lịch (VN) cho ScheduledDate / PaymentDate đã lưu theo quy ước CalendarDateMidnight.</summary>
    public static (DateTime? StartUtc, DateTime? EndExclusiveUtc) ToUtcDayBounds(DateOnly? from, DateOnly? to)
    {
        var (start, endEx) = VietnamTime.DayBounds(from, to);
        return (start, endEx);
    }
}
