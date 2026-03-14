using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, GetOrdersResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetOrdersQueryHandler> _logger;

    public GetOrdersQueryHandler(IOrderRepository orderRepository, ILogger<GetOrdersQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<GetOrdersResponse> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _orderRepository.GetOrdersAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var orderDtos = orders.Select(order => new OrderDto
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
        }).ToList();

        _logger.LogInformation("Retrieved {Count} orders (Page {Page}, PageSize {PageSize})",
            orderDtos.Count, request.Page, request.PageSize);

        return new GetOrdersResponse
        {
            Data = orderDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
