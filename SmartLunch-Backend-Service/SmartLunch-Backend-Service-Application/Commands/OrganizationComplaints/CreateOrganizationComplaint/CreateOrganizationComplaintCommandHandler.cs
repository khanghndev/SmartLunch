using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationComplaints.CreateOrganizationComplaint;

public class CreateOrganizationComplaintCommandHandler
    : IRequestHandler<CreateOrganizationComplaintCommand, OrganizationComplaintDetailDto>
{
    private readonly IComplaintRepository _complaints;
    private readonly IOrderRepository _orders;
    private readonly IUserOrganizationRepository _userOrganizations;
    private readonly IStorageService _storage;

    public CreateOrganizationComplaintCommandHandler(
        IComplaintRepository complaints,
        IOrderRepository orders,
        IUserOrganizationRepository userOrganizations,
        IStorageService storage)
    {
        _complaints = complaints;
        _orders = orders;
        _userOrganizations = userOrganizations;
        _storage = storage;
    }

    public async Task<OrganizationComplaintDetailDto> Handle(CreateOrganizationComplaintCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        var reason = (req.Reason ?? string.Empty).Trim().ToLowerInvariant();
        if (!ComplaintReason.All.Contains(reason))
            throw new ArgumentException("Lý do khiếu nại không hợp lệ.");

        if (string.IsNullOrWhiteSpace(req.Description))
            throw new ArgumentException("Mô tả khiếu nại là bắt buộc.");

        var orgIds = await OrganizationComplaintRules.GetActiveOrganizationIdsAsync(
            _userOrganizations, command.UserId, cancellationToken);
        if (orgIds.Count == 0)
            throw new UnauthorizedAccessException("Chỉ khách hàng doanh nghiệp mới được khiếu nại.");

        var order = await _orders.GetByIdWithDetailsAsync(req.OrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order not found: {req.OrderId}");

        if (!OrganizationComplaintRules.UserCanAccessOrder(order, command.UserId, orgIds))
            throw new UnauthorizedAccessException("Bạn không có quyền với đơn hàng này.");

        var now = VietnamTime.Now;
        if (!OrganizationComplaintRules.CanComplainAboutOrder(order, now))
        {
            var deadline = OrganizationComplaintRules.GetComplaintDeadline(order);
            if (deadline.HasValue && now > deadline.Value)
                throw new InvalidOperationException(
                    $"Đã quá 24 giờ kể từ lúc đơn chuyển sang đã giao (hết hạn lúc {deadline.Value:dd/MM/yyyy HH:mm}).");
            throw new InvalidOperationException("Chỉ được khiếu nại khi đơn đã giao và trong vòng 24 giờ kể từ thời điểm đó.");
        }

        var mainPortions = OrganizationComplaintRules.CountMainPortions(order);
        int? missingCount = null;
        int? refundPortions = null;

        if (string.Equals(reason, ComplaintReason.MissingPortions, StringComparison.OrdinalIgnoreCase))
        {
            missingCount = req.MissingPortionCount ?? req.RefundPortionCount;
            if (missingCount is not > 0)
                throw new ArgumentException("Số suất thiếu phải lớn hơn 0.");
            if (missingCount > mainPortions)
                throw new ArgumentException($"Số suất thiếu không được vượt quá {mainPortions} suất đã đặt.");
            refundPortions = missingCount;
        }
        else if (req.RefundPortionCount is > 0)
        {
            refundPortions = req.RefundPortionCount;
            if (refundPortions > mainPortions)
                throw new ArgumentException($"Số suất hoàn không được vượt quá {mainPortions}.");
        }

        var deadline = OrganizationComplaintRules.GetComplaintDeadline(order);

        var entity = new Complaint
        {
            UserId = command.UserId,
            OrderId = req.OrderId,
            Title = OrganizationComplaintRules.TitleForReason(reason),
            Description = req.Description.Trim(),
            Reason = reason,
            MissingPortionCount = missingCount,
            RefundPortionCount = refundPortions,
            Status = ComplaintStatus.Draft,
            CreatedAt = now,
            ComplaintDeadlineAt = deadline,
            SuggestedRefundAmount = ComplaintRefundCalculator.SuggestRefund(order, refundPortions),
        };

        await _complaints.CreateAsync(entity, cancellationToken);
        var loaded = await _complaints.GetByIdWithDetailsAsync(entity.Id, cancellationToken)
            ?? entity;
        return await ComplaintMapper.ToDetailAsync(loaded, _storage, cancellationToken);
    }
}
