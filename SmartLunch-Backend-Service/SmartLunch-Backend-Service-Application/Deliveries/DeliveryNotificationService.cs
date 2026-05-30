using SmartLunch.Backend.Service.Application.Integration.Email;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Deliveries;

public class DeliveryNotificationService
{
    private readonly INotificationRepository _notifications;
    private readonly DeliveryEmailService _emails;
    private readonly IOrderRepository _orders;

    public DeliveryNotificationService(
        INotificationRepository notifications,
        DeliveryEmailService emails,
        IOrderRepository orders)
    {
        _notifications = notifications;
        _emails = emails;
        _orders = orders;
    }

    public async Task NotifyOrderOwnerDeliveryOtpAsync(
        Delivery delivery,
        int notifyUserId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(delivery.DeliveryOtp))
            return;

        var expires = delivery.DeliveryOtpExpiresAt?.ToString("dd/MM/yyyy HH:mm") ?? "—";
        await _notifications.CreateManyAsync(new[]
        {
            new Notification
            {
                UserId = notifyUserId,
                Title = "Mã xác nhận nhận hàng",
                Message = $"Đơn #{delivery.OrderId}: mã OTP giao hàng là {delivery.DeliveryOtp} (hiệu lực đến {expires}). Cung cấp cho shipper khi nhận suất.",
                Type = "info",
                Link = $"/organization/orders/{delivery.OrderId}",
                SendAt = VietnamTime.Now,
            }
        }, cancellationToken);

        var order = delivery.Order ?? await _orders.GetByIdWithDetailsAsync(delivery.OrderId);
        if (order != null)
            await _emails.TrySendDeliveryOtpAsync(delivery, order, cancellationToken);
    }
}
