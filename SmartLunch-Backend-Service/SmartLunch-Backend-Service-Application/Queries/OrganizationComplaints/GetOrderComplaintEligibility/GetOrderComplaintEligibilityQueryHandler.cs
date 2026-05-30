using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationComplaints.GetOrderComplaintEligibility;

public class GetOrderComplaintEligibilityQueryHandler : IRequestHandler<GetOrderComplaintEligibilityQuery, ComplaintEligibilityDto>
{
    private readonly IOrderRepository _orders;
    private readonly IUserOrganizationRepository _userOrganizations;
    private readonly IComplaintRepository _complaints;

    public GetOrderComplaintEligibilityQueryHandler(
        IOrderRepository orders,
        IUserOrganizationRepository userOrganizations,
        IComplaintRepository complaints)
    {
        _orders = orders;
        _userOrganizations = userOrganizations;
        _complaints = complaints;
    }

    public async Task<ComplaintEligibilityDto> Handle(GetOrderComplaintEligibilityQuery request, CancellationToken cancellationToken)
    {
        var orgIds = await OrganizationComplaintRules.GetActiveOrganizationIdsAsync(
            _userOrganizations, request.UserId, cancellationToken);
        if (orgIds.Count == 0)
            throw new UnauthorizedAccessException("Chỉ khách hàng doanh nghiệp mới được khiếu nại.");

        var order = await _orders.GetByIdWithDetailsAsync(request.OrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order not found: {request.OrderId}");

        if (!OrganizationComplaintRules.UserCanAccessOrder(order, request.UserId, orgIds))
            throw new UnauthorizedAccessException("Bạn không có quyền với đơn hàng này.");

        var now = VietnamTime.Now;
        var deliveredAt = OrganizationComplaintRules.GetDeliveredAt(order);
        var deadline = OrganizationComplaintRules.GetComplaintDeadline(order);
        var canComplain = OrganizationComplaintRules.CanComplainAboutOrder(order, now);

        string? blockReason = null;
        if (!canComplain)
        {
            if (deliveredAt == null)
                blockReason = "Đơn chưa được giao thành công.";
            else if (deadline.HasValue && now > deadline.Value)
                blockReason = "Đã hết thời hạn khiếu nại (24 giờ sau khi giao).";
            else
                blockReason = "Đơn chưa đủ điều kiện khiếu nại.";
        }

        var existingCount = await _complaints.CountByOrderAndUserAsync(request.OrderId, request.UserId, cancellationToken);

        return new ComplaintEligibilityDto
        {
            OrderId = request.OrderId,
            CanComplain = canComplain,
            BlockReason = blockReason,
            DeliveredAt = deliveredAt,
            ComplaintDeadlineAt = deadline,
            OrderedMainPortionCount = OrganizationComplaintRules.CountMainPortions(order),
            ExistingComplaintCount = existingCount,
        };
    }
}
