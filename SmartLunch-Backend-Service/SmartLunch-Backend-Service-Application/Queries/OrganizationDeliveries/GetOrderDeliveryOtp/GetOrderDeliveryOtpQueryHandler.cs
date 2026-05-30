using MediatR;
using SmartLunch.Backend.Service.Application.Deliveries;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationDeliveries;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationReviews;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationDeliveries.GetOrderDeliveryOtp;

public class GetOrderDeliveryOtpQueryHandler : IRequestHandler<GetOrderDeliveryOtpQuery, OrganizationDeliveryOtpResponse>
{
    private readonly IOrderRepository _orders;
    private readonly IUserOrganizationRepository _userOrganizations;

    public GetOrderDeliveryOtpQueryHandler(IOrderRepository orders, IUserOrganizationRepository userOrganizations)
    {
        _orders = orders;
        _userOrganizations = userOrganizations;
    }

    public async Task<OrganizationDeliveryOtpResponse> Handle(GetOrderDeliveryOtpQuery request, CancellationToken cancellationToken)
    {
        var orgIds = await OrganizationReviewRules.GetActiveOrganizationIdsAsync(
            _userOrganizations, request.UserId, cancellationToken);
        if (orgIds.Count == 0)
            throw new UnauthorizedAccessException("Chỉ khách hàng doanh nghiệp mới xem được mã OTP.");

        var order = await _orders.GetByIdWithDetailsAsync(request.OrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order not found: {request.OrderId}");

        if (!OrganizationReviewRules.UserCanAccessOrder(order, request.UserId, orgIds))
            throw new UnauthorizedAccessException("Bạn không có quyền với đơn hàng này.");

        var delivery = order.Deliveries?
            .OrderByDescending(d => d.Id)
            .FirstOrDefault();

        if (delivery == null)
            throw new InvalidOperationException("Đơn chưa có thông tin giao hàng.");

        return new OrganizationDeliveryOtpResponse
        {
            OrderId = request.OrderId,
            DeliveryId = delivery.Id,
            DeliveryOtp = delivery.DeliveryOtp,
            DeliveryOtpExpiresAt = delivery.DeliveryOtpExpiresAt,
            DeliveryStatus = delivery.DeliveryStatus,
            IsActive = DeliveryOtpService.IsOtpActive(delivery),
        };
    }
}
