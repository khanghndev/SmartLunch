using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.OrderItems;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.OrderItems.GetOrderItem;

public class GetOrderItemQueryHandler : IRequestHandler<GetOrderItemQuery, GetOrderItemResponse>
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ILogger<GetOrderItemQueryHandler> _logger;

    public GetOrderItemQueryHandler(IOrderItemRepository orderItemRepository, ILogger<GetOrderItemQueryHandler> logger)
    {
        _orderItemRepository = orderItemRepository;
        _logger = logger;
    }

    public async Task<GetOrderItemResponse> Handle(GetOrderItemQuery request, CancellationToken cancellationToken)
    {
        var orderItem = await _orderItemRepository.GetByIdAsync(request.OrderItemId);

        if (orderItem == null)
        {
            _logger.LogWarning("OrderItem not found with ID: {OrderItemId}", request.OrderItemId);
            return new GetOrderItemResponse { OrderItem = new OrderItemDto() };
        }

        return new GetOrderItemResponse
        {
            OrderItem = new OrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                DishId = orderItem.DishId,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.TotalPrice
            }
        };
    }
}
