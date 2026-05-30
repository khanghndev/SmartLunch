using SmartLunch.Backend.Service.Application.Integration.Email;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationComplaints;

public class ComplaintNotificationService
{
    private readonly INotificationRepository _notifications;
    private readonly IUserRepository _users;
    private readonly ComplaintEmailService _emails;

    public ComplaintNotificationService(
        INotificationRepository notifications,
        IUserRepository users,
        ComplaintEmailService emails)
    {
        _notifications = notifications;
        _users = users;
        _emails = emails;
    }

    public async Task NotifyManagersNewComplaintAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        var managerIds = await GetManagerUserIdsAsync(cancellationToken);
        if (managerIds.Count == 0)
            return;

        var label = string.IsNullOrWhiteSpace(complaint.Code) ? $"#{complaint.Id}" : complaint.Code;
        var now = VietnamTime.Now;
        var items = managerIds.Select(uid => new Notification
        {
            UserId = uid,
            Title = "Khiếu nại mới cần xử lý",
            Message = $"Khiếu nại {label} cho đơn #{complaint.OrderId} đã được gửi.",
            Type = "warning",
            Link = $"/manager/complaints/{complaint.Id}",
            SendAt = now,
        });

        await _notifications.CreateManyAsync(items, cancellationToken);
        await _emails.TrySendNewComplaintToManagersAsync(complaint, cancellationToken);
    }

    public async Task NotifyComplainantResolvedAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        var label = string.IsNullOrWhiteSpace(complaint.Code) ? $"#{complaint.Id}" : complaint.Code;
        var isRefund = string.Equals(complaint.Resolution, Constants.ComplaintResolution.Refund, StringComparison.OrdinalIgnoreCase);
        var amountText = complaint.FinalRefundAmount is > 0
            ? $" Số tiền hoàn: {complaint.FinalRefundAmount:N0} đ."
            : string.Empty;

        var message = isRefund
            ? $"Khiếu nại {label} đã được chấp nhận hoàn tiền.{amountText}"
            : $"Khiếu nại {label} đã bị từ chối.{(string.IsNullOrWhiteSpace(complaint.ResolutionNote) ? "" : " Lý do: " + complaint.ResolutionNote)}";

        await _notifications.CreateManyAsync(new[]
        {
            new Notification
            {
                UserId = complaint.UserId,
                Title = isRefund ? "Khiếu nại đã được hoàn tiền" : "Khiếu nại bị từ chối",
                Message = message,
                Type = isRefund ? "success" : "info",
                Link = $"/organization/complaints/{complaint.Id}",
                SendAt = VietnamTime.Now,
            }
        }, cancellationToken);
        await _emails.TrySendResolvedToComplainantAsync(complaint, cancellationToken);
    }

    private async Task<List<int>> GetManagerUserIdsAsync(CancellationToken cancellationToken)
    {
        var ids = new HashSet<int>();
        foreach (var role in new[] { "Manager", "Admin" })
        {
            var (users, _) = await _users.GetUsersAsync(1, 500, roleName: role, isActive: true);
            foreach (var u in users)
                ids.Add(u.Id);
        }

        return ids.ToList();
    }
}
