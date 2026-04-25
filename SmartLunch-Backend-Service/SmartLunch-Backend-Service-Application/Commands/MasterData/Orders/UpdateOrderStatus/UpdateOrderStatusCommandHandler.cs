using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, GetOrderResponse>
{
    private const string DeliveryCompleted = "completed";

    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<GetOrderResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var target = (request.Request.Status ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(target))
            throw new ArgumentException("Status is required.");

        if (target is not (
            OrderLifecycleStatus.Pending or
            OrderLifecycleStatus.Confirmed or
            OrderLifecycleStatus.Delivered))
        {
            throw new ArgumentException(
                $"Invalid status '{request.Request.Status}'. Allowed values: {OrderLifecycleStatus.Pending}, {OrderLifecycleStatus.Confirmed}, {OrderLifecycleStatus.Delivered}.");
        }

        var order = await _orderRepository.GetByIdWithDetailsAsync(request.OrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {request.OrderId} was not found.");

        var current = order.Status.Trim().ToLowerInvariant();
        if (!IsAllowedTransition(current, target))
        {
            throw new ArgumentException(
                $"Cannot change order status from '{current}' to '{target}'. Allowed flow: {OrderLifecycleStatus.Pending} → {OrderLifecycleStatus.Confirmed} → {OrderLifecycleStatus.Delivered}.");
        }

        if (current != target)
        {
            order.Status = target;
            order.UpdatedAt = DateTime.UtcNow;
        }

        if (target == OrderLifecycleStatus.Delivered)
            EnsureDeliveredDelivery(order);

        await _orderRepository.CommitAsync();

        var refreshed = await _orderRepository.GetByIdWithDetailsAsync(request.OrderId);
        if (refreshed == null)
        {
            _logger.LogWarning("Order disappeared after update: {OrderId}", request.OrderId);
            return new GetOrderResponse { Order = new OrderDto() };
        }

        return new GetOrderResponse { Order = OrderDtoMapping.ToDto(refreshed) };
    }

    private static bool IsAllowedTransition(string current, string target)
    {
        if (string.Equals(current, target, StringComparison.Ordinal))
            return true;

        if (current == OrderLifecycleStatus.Cancelled)
            return false;

        return (current, target) switch
        {
            (OrderLifecycleStatus.Pending, OrderLifecycleStatus.Confirmed) => true,
            (OrderLifecycleStatus.Confirmed, OrderLifecycleStatus.Delivered) => true,
            (OrderLifecycleStatus.Preparing, OrderLifecycleStatus.Delivered) => true,
            (OrderLifecycleStatus.Preparing, OrderLifecycleStatus.Confirmed) => true,
            _ => false
        };
    }

    private static void EnsureDeliveredDelivery(Order order)
    {
        var completed = order.Deliveries.FirstOrDefault(d =>
            string.Equals(d.DeliveryStatus, DeliveryCompleted, StringComparison.OrdinalIgnoreCase));
        if (completed != null)
        {
            completed.DeliveredAt ??= DateTime.UtcNow;
            return;
        }

        var delivery = order.Deliveries
            .OrderBy(d => d.CreatedAt)
            .FirstOrDefault();

        if (delivery != null)
        {
            delivery.DeliveryStatus = DeliveryCompleted;
            delivery.DeliveredAt = DateTime.UtcNow;
            return;
        }

        var address = order.Unit?.Address;
        if (string.IsNullOrWhiteSpace(address))
            address = order.Unit?.Name;
        if (string.IsNullOrWhiteSpace(address))
            address = "—";

        order.Deliveries.Add(new Delivery
        {

            OrderId = order.Id,
            DeliveryAddress = address,
            DeliveryStatus = DeliveryCompleted,
            DeliveredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
    }
}
