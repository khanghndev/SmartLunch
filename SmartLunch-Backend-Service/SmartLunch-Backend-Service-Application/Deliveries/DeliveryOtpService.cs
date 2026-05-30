using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Deliveries;

public static class DeliveryOtpService
{
    public const int OtpValidityHours = 48;

    public static string GenerateAndAssign(Delivery delivery)
    {
        var otp = GenerateCode();
        var now = VietnamTime.Now;
        delivery.DeliveryOtp = otp;
        delivery.DeliveryOtpExpiresAt = now.AddHours(OtpValidityHours);
        return otp;
    }

    public static void ValidateForProof(Delivery delivery, string? confirmationCode)
    {
        var code = (confirmationCode ?? string.Empty).Trim();
        if (code.Length == 0)
            throw new ArgumentException("Mã xác nhận người nhận (OTP) là bắt buộc.");

        var expected = (delivery.DeliveryOtp ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(expected))
            throw new InvalidOperationException("Chưa có mã OTP giao hàng. Shipper cần chuyển trạng thái sang đang giao (in_transit) trước.");

        if (delivery.DeliveryOtpExpiresAt.HasValue && VietnamTime.Now > delivery.DeliveryOtpExpiresAt.Value)
            throw new InvalidOperationException("Mã OTP giao hàng đã hết hạn.");

        if (!string.Equals(expected, code, StringComparison.Ordinal))
            throw new ArgumentException("Mã OTP không đúng. Vui lòng xác nhận lại với người nhận.");
    }

    public static bool IsOtpActive(Delivery delivery)
    {
        if (string.IsNullOrWhiteSpace(delivery.DeliveryOtp))
            return false;
        if (delivery.DeliveryOtpExpiresAt.HasValue && VietnamTime.Now > delivery.DeliveryOtpExpiresAt.Value)
            return false;
        return true;
    }

    private static string GenerateCode()
    {
        var value = Random.Shared.Next(0, 1_000_000);
        return value.ToString("D6");
    }
}
