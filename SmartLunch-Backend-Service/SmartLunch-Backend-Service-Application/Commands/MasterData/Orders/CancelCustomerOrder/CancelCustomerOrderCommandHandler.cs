using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Integration.PayOS;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Orders;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.CancelCustomerOrder;

public class CancelCustomerOrderCommandHandler : IRequestHandler<CancelCustomerOrderCommand, GetOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPayOSClient _payOSClient;
    private readonly ILogger<CancelCustomerOrderCommandHandler> _logger;

    public CancelCustomerOrderCommandHandler(
        IOrderRepository orderRepository,
        IPayOSClient payOSClient,
        ILogger<CancelCustomerOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _payOSClient = payOSClient;
        _logger = logger;
    }

    public async Task<GetOrderResponse> Handle(CancelCustomerOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(request.OrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} was not found.");

        if (order.UserId != request.UserId)
            throw new UnauthorizedAccessException("Bạn chỉ được hủy đơn hàng của chính mình.");

        var (canCancel, blockReason) = OrderCustomerCancelEligibility.Evaluate(order);
        if (!canCancel)
            throw new InvalidOperationException(blockReason ?? "Không thể hủy đơn hàng này.");

        await CancelPendingPayOsSessionsAsync(order, cancellationToken);

        order.Status = OrderLifecycleStatus.Cancelled;
        order.UpdatedAt = VietnamTime.Now;

        foreach (var delivery in order.Deliveries)
        {
            if (!string.Equals(delivery.DeliveryStatus, "completed", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(delivery.DeliveryStatus, "cancelled", StringComparison.OrdinalIgnoreCase))
            {
                delivery.DeliveryStatus = "cancelled";
            }
        }

        await _orderRepository.CommitAsync();

        var refreshed = await _orderRepository.GetByIdWithDetailsAsync(request.OrderId);
        if (refreshed == null)
            return new GetOrderResponse { Order = new OrderDto() };

        return new GetOrderResponse { Order = OrderDtoMapping.ToDto(refreshed) };
    }

    private async Task CancelPendingPayOsSessionsAsync(Order order, CancellationToken cancellationToken)
    {
        var pendingPayOs = order.Payments
            .Where(p =>
                string.Equals(p.Method, "payos", StringComparison.OrdinalIgnoreCase)
                && string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var payment in pendingPayOs)
        {
            try
            {
                await _payOSClient.CancelPaymentRequestAsync(
                    payment.Id,
                    "Khách hủy đơn hàng",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "PayOS cancel failed for payment {PaymentId} on order {OrderId}; marking payment cancelled locally.",
                    payment.Id,
                    order.Id);
            }

            payment.Status = "cancelled";
        }
    }
}
