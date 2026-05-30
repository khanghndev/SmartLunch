using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationComplaints.SubmitOrganizationComplaint;

public class SubmitOrganizationComplaintCommandHandler
    : IRequestHandler<SubmitOrganizationComplaintCommand, OrganizationComplaintDetailDto>
{
    private readonly IComplaintRepository _complaints;
    private readonly IOrderRepository _orders;
    private readonly IStorageService _storage;
    private readonly ComplaintNotificationService _notifications;

    public SubmitOrganizationComplaintCommandHandler(
        IComplaintRepository complaints,
        IOrderRepository orders,
        IStorageService storage,
        ComplaintNotificationService notifications)
    {
        _complaints = complaints;
        _orders = orders;
        _storage = storage;
        _notifications = notifications;
    }

    public async Task<OrganizationComplaintDetailDto> Handle(SubmitOrganizationComplaintCommand command, CancellationToken cancellationToken)
    {
        var complaint = await _complaints.GetByIdWithDetailsAsync(command.ComplaintId, cancellationToken);
        if (complaint == null)
            throw new KeyNotFoundException("Khiếu nại không tồn tại.");
        if (complaint.UserId != command.UserId)
            throw new UnauthorizedAccessException("Không có quyền với khiếu nại này.");
        if (!OrganizationComplaintRules.IsEditable(complaint))
            throw new InvalidOperationException("Chỉ được gửi khiếu nại ở trạng thái nháp.");

        if (complaint.OrderId is not int orderId)
            throw new InvalidOperationException("Khiếu nại không gắn với đơn hàng.");

        var order = await _orders.GetByIdWithDetailsAsync(orderId);
        if (order == null)
            throw new KeyNotFoundException("Đơn hàng không tồn tại.");

        var now = VietnamTime.Now;
        if (!OrganizationComplaintRules.CanComplainAboutOrder(order, now))
            throw new InvalidOperationException("Đã hết thời hạn khiếu nại 24 giờ sau khi giao.");

        ComplaintEvidenceValidator.ValidateForSubmit(complaint);

        complaint.Status = ComplaintStatus.PendingReview;
        complaint.SubmittedAt = now;
        complaint.SuggestedRefundAmount = ComplaintRefundCalculator.SuggestRefund(order, complaint.RefundPortionCount);

        await _complaints.UpdateAsync(complaint, cancellationToken);
        await _complaints.CommitAsync(cancellationToken);

        await _notifications.NotifyManagersNewComplaintAsync(complaint, cancellationToken);

        var reloaded = await _complaints.GetByIdWithDetailsAsync(complaint.Id, cancellationToken) ?? complaint;
        return await ComplaintMapper.ToDetailAsync(reloaded, _storage, cancellationToken);
    }
}
