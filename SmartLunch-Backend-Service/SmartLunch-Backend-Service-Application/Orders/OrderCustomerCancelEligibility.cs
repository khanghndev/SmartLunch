using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Orders;

/// <summary>Quy tắc khách được phép tự hủy đơn trên cổng B2C/B2B.</summary>
public static class OrderCustomerCancelEligibility
{
    public static (bool CanCancel, string? BlockReason) Evaluate(Order order)
    {
        var status = (order.Status ?? string.Empty).Trim().ToLowerInvariant();

        if (status == OrderLifecycleStatus.Cancelled)
            return (false, "Đơn hàng đã được hủy trước đó.");

        if (status == OrderLifecycleStatus.Delivered)
            return (false, "Đơn đã giao hàng, không thể hủy trực tuyến. Vui lòng liên hệ hỗ trợ nếu cần khiếu nại.");

        if (status is OrderLifecycleStatus.Preparing or OrderLifecycleStatus.Confirmed)
            return (false, "Đơn đang được xử lý. Vui lòng liên hệ bộ phận hỗ trợ để được hủy.");

        if (status != OrderLifecycleStatus.Pending)
            return (false, "Chỉ có thể hủy đơn đang chờ xác nhận.");

        var payment = (order.PaymentStatus ?? string.Empty).Trim().ToLowerInvariant();
        if (payment is OrderPaymentStatus.DepositPaid or OrderPaymentStatus.Partial or OrderPaymentStatus.Paid)
            return (false, "Đơn đã có khoản thanh toán. Vui lòng liên hệ hỗ trợ để được hoàn tiền.");

        if (DateOnly.FromDateTime(order.ScheduledDate) < VietnamTime.Today)
            return (false, "Đơn đã qua ngày giao dự kiến, không thể hủy trực tuyến.");

        return (true, null);
    }
}
