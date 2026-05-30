using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.ManagerComplaints.ResolveManagerComplaint;

public class ResolveManagerComplaintCommandHandler : IRequestHandler<ResolveManagerComplaintCommand, ManagerComplaintDetailDto>
{
    private readonly IComplaintRepository _complaints;
    private readonly IOrderRepository _orders;
    private readonly IStorageService _storage;
    private readonly ComplaintRefundRecorder _refundRecorder;
    private readonly ComplaintNotificationService _notifications;

    public ResolveManagerComplaintCommandHandler(
        IComplaintRepository complaints,
        IOrderRepository orders,
        IStorageService storage,
        ComplaintRefundRecorder refundRecorder,
        ComplaintNotificationService notifications)
    {
        _complaints = complaints;
        _orders = orders;
        _storage = storage;
        _refundRecorder = refundRecorder;
        _notifications = notifications;
    }

    public async Task<ManagerComplaintDetailDto> Handle(ResolveManagerComplaintCommand command, CancellationToken cancellationToken)
    {
        var resolution = (command.Request.Resolution ?? string.Empty).Trim().ToLowerInvariant();
        if (resolution is not (ComplaintResolution.Refund or ComplaintResolution.Rejected))
            throw new ArgumentException("Quyết định phải là refund hoặc rejected.");

        if (resolution == ComplaintResolution.Rejected && string.IsNullOrWhiteSpace(command.Request.ResolutionNote))
            throw new ArgumentException("Cần ghi chú khi từ chối khiếu nại.");

        var complaint = await _complaints.GetByIdWithDetailsAsync(command.ComplaintId, cancellationToken);
        if (complaint == null)
            throw new KeyNotFoundException("Khiếu nại không tồn tại.");

        var status = (complaint.Status ?? string.Empty).ToLowerInvariant();
        if (status is not (ComplaintStatus.PendingReview or ComplaintStatus.LegacyNew or ComplaintStatus.LegacyInProgress))
            throw new InvalidOperationException("Khiếu nại không ở trạng thái chờ xử lý.");

        var now = VietnamTime.Now;
        complaint.Resolution = resolution;
        complaint.ResolutionNote = command.Request.ResolutionNote?.Trim();
        complaint.ResolvedByUserId = command.ManagerUserId;
        complaint.ResolvedAt = now;
        complaint.Status = resolution == ComplaintResolution.Refund
            ? ComplaintStatus.Resolved
            : ComplaintStatus.Rejected;

        if (resolution == ComplaintResolution.Refund)
        {
            Order? order = complaint.Order;
            if (order == null && complaint.OrderId is int oid)
                order = await _orders.GetByIdWithDetailsAsync(oid);

            var portions = command.Request.RefundPortionCount ?? complaint.RefundPortionCount ?? complaint.MissingPortionCount;
            if (portions is > 0)
                complaint.RefundPortionCount = portions;

            var suggested = order != null
                ? ComplaintRefundCalculator.SuggestRefund(order, complaint.RefundPortionCount)
                : complaint.SuggestedRefundAmount;
            complaint.SuggestedRefundAmount = suggested;

            if (command.Request.FinalRefundAmount is > 0)
                complaint.FinalRefundAmount = decimal.Round(command.Request.FinalRefundAmount.Value, 2, MidpointRounding.AwayFromZero);
            else if (suggested is > 0)
                complaint.FinalRefundAmount = suggested;
            else
                throw new ArgumentException("Cần nhập số tiền hoàn (FinalRefundAmount) hoặc số suất hoàn.");

            await _refundRecorder.RecordAsync(complaint, order, cancellationToken);
        }
        else
        {
            complaint.FinalRefundAmount = null;
        }

        await _complaints.UpdateAsync(complaint, cancellationToken);
        await _complaints.CommitAsync(cancellationToken);

        await _notifications.NotifyComplainantResolvedAsync(complaint, cancellationToken);

        var reloaded = await _complaints.GetByIdWithDetailsAsync(complaint.Id, cancellationToken) ?? complaint;
        return await ComplaintMapper.ToManagerDetailAsync(reloaded, _storage, cancellationToken);
    }
}
