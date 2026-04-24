namespace SmartLunch.Backend.Service.Application.Helpers;

public static class CutOffTimeValidator
{
    /// <summary>
    /// Checks whether an order is allowed to be placed based on the UnitType and ScheduledDate.
    /// Throws ArgumentException if the cut-off time is violated.
    /// </summary>
    /// <param name="unitType">"Office", "Factory", "School"</param>
    /// <param name="scheduledDate">The desired date/time for the meal</param>
    /// <param name="nowUtc">The current time in UTC</param>
    public static void Validate(string unitType, DateTime scheduledDate, DateTime nowUtc)
    {
        // Convert UTC to Vietnam Time (+7) for accurate logic
        var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var nowVn = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, vnTimeZone);
        var scheduledVn = scheduledDate.Kind == DateTimeKind.Utc 
            ? TimeZoneInfo.ConvertTimeFromUtc(scheduledDate, vnTimeZone) 
            : scheduledDate; // Assuming scheduled date is already passed in local equivalent if not UTC

        var daysDifference = (scheduledVn.Date - nowVn.Date).Days;

        switch (unitType.ToLowerInvariant())
        {
            case "office":
                // Trước >= 1 ngày, chốt đơn lúc 17:00 ngày liền trước
                if (daysDifference < 1)
                    throw new ArgumentException("Đối với Văn phòng, không thể đặt đơn cho ngày hôm nay.");
                if (daysDifference == 1 && nowVn.Hour >= 17)
                    throw new ArgumentException("Đối với Văn phòng, bạn phải chốt đơn đặt trước 17:00 ngày hôm trước.");
                break;

            case "factory":
                // Trước >= 2 ngày (chấp nhận 1-2 ngày tuỳ config, fix logic 48h)
                if (daysDifference < 2)
                    throw new ArgumentException("Đối với Xí nghiệp, đơn hàng phải được đặt trước ít nhất 2 ngày bảo đảm khâu chuẩn bị.");
                break;

            case "school":
                // Trước >= 3 ngày
                if (daysDifference < 3)
                    throw new ArgumentException("Đối với Trường học, đơn hàng phải được lên trước ít nhất 3 ngày theo quy định an toàn thực phẩm.");
                break;

            default:
                // Nếu unit type không xác định, có thể cho phép hoặc fallback logic chung
                if (daysDifference < 1)
                    throw new ArgumentException("Phải đặt đơn trước ít nhất 1 ngày.");
                break;
        }
    }
}
