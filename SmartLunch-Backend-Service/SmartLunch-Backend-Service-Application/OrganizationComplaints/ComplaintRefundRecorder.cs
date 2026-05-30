using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationComplaints;

/// <summary>Ghi nhận hoàn tiền: Payment (refunded) + Transaction (chi).</summary>
public class ComplaintRefundRecorder
{
    private readonly IPaymentRepository _payments;
    private readonly ITransactionRepository _transactions;
    private readonly IOrderRepository _orders;

    public ComplaintRefundRecorder(
        IPaymentRepository payments,
        ITransactionRepository transactions,
        IOrderRepository orders)
    {
        _payments = payments;
        _transactions = transactions;
        _orders = orders;
    }

    public async Task RecordAsync(Complaint complaint, Order? order = null, CancellationToken cancellationToken = default)
    {
        if (complaint.RefundPaymentId.HasValue)
            throw new InvalidOperationException("Khiếu nại đã được ghi nhận hoàn tiền.");

        if (complaint.OrderId is not int orderId)
            throw new InvalidOperationException("Khiếu nại không gắn đơn hàng.");

        var amount = complaint.FinalRefundAmount;
        if (amount is not > 0)
            throw new InvalidOperationException("Số tiền hoàn phải lớn hơn 0.");

        var now = VietnamTime.Now;
        var rounded = decimal.Round(amount.Value, 2, MidpointRounding.AwayFromZero);

        var payment = new Payment
        {
            OrderId = orderId,
            PayerId = complaint.UserId,
            PaymentDate = now,
            Amount = rounded,
            Method = PaymentMethod.ComplaintRefund,
            Status = PaymentStatus.Refunded,
            CreatedAt = now,
        };

        var createdPayment = await _payments.CreateForOrderAsync(payment, cancellationToken);
        complaint.RefundPaymentId = createdPayment.Id;

        var label = string.IsNullOrWhiteSpace(complaint.Code) ? $"#{complaint.Id}" : complaint.Code;
        await _transactions.CreateAsync(new Transaction
        {
            Date = now,
            Description = $"Hoàn tiền khiếu nại {label} — đơn #{orderId}",
            Amount = -rounded,
            Category = TransactionCategory.ComplaintRefund,
            Method = PaymentMethod.ComplaintRefund,
            ReferenceId = complaint.Id,
            CreatedAt = now,
        }, cancellationToken);

        order ??= await _orders.GetByIdWithDetailsAsync(orderId);
        if (order != null)
        {
            if (!order.Payments.Any(p => p.Id == createdPayment.Id))
                order.Payments.Add(createdPayment);
            ComplaintOrderPaymentAdjuster.ApplyAfterRefund(order);
            await _orders.CommitAsync();
        }
    }
}
