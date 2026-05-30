using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Integration.Email;

public class ComplaintEmailService
{
    private readonly IEmailSender _email;
    private readonly IUserRepository _users;
    private readonly ILogger<ComplaintEmailService> _logger;

    public ComplaintEmailService(IEmailSender email, IUserRepository users, ILogger<ComplaintEmailService> logger)
    {
        _email = email;
        _users = users;
        _logger = logger;
    }

    public async Task TrySendNewComplaintToManagersAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        var label = FormatLabel(complaint);
        var subject = $"[SmartLunch] Khiếu nại mới {label}";
        var body = $"""
            <p>Khiếu nại <strong>{label}</strong> cho đơn hàng #{complaint.OrderId} cần được xử lý.</p>
            <p>Lý do: {complaint.Reason ?? "—"}</p>
            <p>Đăng nhập ứng dụng Quản lý để xem chi tiết.</p>
            """;

        await SendToRoleUsersAsync(new[] { "Manager", "Admin" }, subject, body, cancellationToken);
    }

    public async Task TrySendResolvedToComplainantAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(complaint.UserId);
        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return;

        var label = FormatLabel(complaint);
        var isRefund = string.Equals(complaint.Resolution, ComplaintResolution.Refund, StringComparison.OrdinalIgnoreCase);
        var subject = isRefund
            ? $"[SmartLunch] Khiếu nại {label} — đã hoàn tiền"
            : $"[SmartLunch] Khiếu nại {label} — đã từ chối";

        var body = isRefund
            ? $"<p>Khiếu nại <strong>{label}</strong> đã được chấp nhận.</p><p>Số tiền hoàn: <strong>{complaint.FinalRefundAmount:N0} đ</strong>.</p>"
            : $"<p>Khiếu nại <strong>{label}</strong> đã bị từ chối.</p><p>Lý do: {complaint.ResolutionNote ?? "—"}</p>";

        await TrySendAsync(user.Email, subject, body, cancellationToken);
    }

    private async Task SendToRoleUsersAsync(
        IEnumerable<string> roles,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var role in roles)
        {
            var (users, _) = await _users.GetUsersAsync(1, 500, roleName: role, isActive: true);
            foreach (var u in users)
            {
                if (!string.IsNullOrWhiteSpace(u.Email))
                    emails.Add(u.Email.Trim());
            }
        }

        foreach (var email in emails)
            await TrySendAsync(email, subject, htmlBody, cancellationToken);
    }

    private async Task TrySendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        try
        {
            await _email.SendAsync(to, subject, htmlBody, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send email to {Email} subject {Subject}", to, subject);
        }
    }

    private static string FormatLabel(Complaint c) =>
        string.IsNullOrWhiteSpace(c.Code) ? $"#{c.Id}" : c.Code!;
}
