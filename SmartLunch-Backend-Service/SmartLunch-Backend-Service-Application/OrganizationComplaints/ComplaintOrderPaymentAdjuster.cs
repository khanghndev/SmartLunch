using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationComplaints;

/// <summary>Cập nhật Order.PaymentStatus sau khi ghi nhận hoàn tiền khiếu nại.</summary>
public static class ComplaintOrderPaymentAdjuster
{
    public static void ApplyAfterRefund(Order order)
    {
        var payments = order.Payments?.ToList() ?? [];
        var paid = payments
            .Where(p => string.Equals(p.Status, PaymentStatus.Paid, StringComparison.OrdinalIgnoreCase))
            .Sum(p => p.Amount);
        var refunded = payments
            .Where(p => string.Equals(p.Status, PaymentStatus.Refunded, StringComparison.OrdinalIgnoreCase))
            .Sum(p => p.Amount);

        var net = paid - refunded;
        var total = order.TotalAmount;

        order.PaymentStatus = net switch
        {
            <= 0 => OrderPaymentStatus.Unpaid,
            var n when n < total => OrderPaymentStatus.Partial,
            _ => OrderPaymentStatus.Paid,
        };
        order.UpdatedAt = VietnamTime.Now;
    }
}
