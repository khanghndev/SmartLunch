using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class CutOffTimeValidator
{
    /// <summary>
    /// Kiểm tra đặt đơn theo loại đơn vị và ngày phục vụ (giờ Việt Nam).
    /// </summary>
    public static void Validate(string organizationType, DateTime scheduledDate, DateTime? nowVietnam = null)
    {
        var nowVn = nowVietnam ?? VietnamTime.Now;
        var scheduledVn = scheduledDate.Kind == DateTimeKind.Utc
            ? VietnamTime.FromUtc(scheduledDate)
            : scheduledDate;

        var daysDifference = (scheduledVn.Date - nowVn.Date).Days;

        switch (organizationType.ToLowerInvariant())
        {
            case "office":
                if (daysDifference < 1)
                    throw new ArgumentException("Đối với Văn phòng, không thể đặt đơn cho ngày hôm nay.");
                if (daysDifference == 1 && nowVn.Hour >= 17)
                    throw new ArgumentException("Đối với Văn phòng, bạn phải chốt đơn đặt trước 17:00 ngày hôm trước.");
                break;

            case "factory":
                if (daysDifference < 2)
                    throw new ArgumentException("Đối với Xí nghiệp, đơn hàng phải được đặt trước ít nhất 2 ngày bảo đảm khâu chuẩn bị.");
                break;

            case "school":
                if (daysDifference < 3)
                    throw new ArgumentException("Đối với Trường học, đơn hàng phải được lên trước ít nhất 3 ngày theo quy định an toàn thực phẩm.");
                break;

            default:
                if (daysDifference < 1)
                    throw new ArgumentException("Phải đặt đơn trước ít nhất 1 ngày.");
                break;
        }
    }
}
