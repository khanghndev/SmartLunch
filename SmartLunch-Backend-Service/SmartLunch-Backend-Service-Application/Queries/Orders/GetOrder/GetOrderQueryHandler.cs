using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetOrder;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetOrderQueryHandler> _logger;

    public GetOrderQueryHandler(IOrderRepository orderRepository, ILogger<GetOrderQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<GetOrderResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);

        if (order == null)
        {
            _logger.LogWarning("Order not found with ID: {OrderId}", request.OrderId);
            return new GetOrderResponse { Order = new OrderDto() };
        }

        return new GetOrderResponse
        {
            Order = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UnitId = order.UnitId,
                OrderDate = order.OrderDate,
                ScheduledDate = order.ScheduledDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                PaymentStatus = order.PaymentStatus,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            }
        };
    }
}
