using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Integration.Email;

public class DeliveryEmailService
{
    private readonly IEmailSender _email;
    private readonly IUserRepository _users;
    private readonly ILogger<DeliveryEmailService> _logger;

    public DeliveryEmailService(IEmailSender email, IUserRepository users, ILogger<DeliveryEmailService> logger)
    {
        _email = email;
        _users = users;
        _logger = logger;
    }

    public async Task TrySendDeliveryOtpAsync(Delivery delivery, Order order, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(delivery.DeliveryOtp))
            return;

        var recipients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var user = await _users.GetByIdAsync(order.UserId ?? 0);
        if (!string.IsNullOrWhiteSpace(user?.Email))
            recipients.Add(user.Email.Trim());
        if (!string.IsNullOrWhiteSpace(order.RecipientEmail))
            recipients.Add(order.RecipientEmail.Trim());

        if (recipients.Count == 0)
            return;

        var expires = delivery.DeliveryOtpExpiresAt?.ToString("dd/MM/yyyy HH:mm") ?? "—";
        var subject = $"[SmartLunch] Mã OTP nhận hàng — đơn #{order.Id}";
        var body = $"""
            <p>Shipper đang giao đơn hàng <strong>#{order.Id}</strong>.</p>
            <p>Mã OTP xác nhận khi nhận suất: <strong style="font-size:20px">{delivery.DeliveryOtp}</strong></p>
            <p>Hiệu lực đến: {expires}</p>
            <p>Vui lòng cung cấp mã này cho nhân viên giao hàng khi nhận.</p>
            """;

        foreach (var to in recipients)
        {
            try
            {
                await _email.SendAsync(to, subject, body, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send delivery OTP email to {Email}", to);
            }
        }
    }
}
