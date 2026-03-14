using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.OrderItems;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.OrderItems.GetOrderItems;

public class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, GetOrderItemsResponse>
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ILogger<GetOrderItemsQueryHandler> _logger;

    public GetOrderItemsQueryHandler(IOrderItemRepository orderItemRepository, ILogger<GetOrderItemsQueryHandler> logger)
    {
        _orderItemRepository = orderItemRepository;
        _logger = logger;
    }

    public async Task<GetOrderItemsResponse> Handle(GetOrderItemsQuery request, CancellationToken cancellationToken)
    {
        var (orderItems, totalCount) = await _orderItemRepository.GetOrderItemsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var orderItemDtos = orderItems.Select(orderItem => new OrderItemDto
        {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                DishId = orderItem.DishId,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.TotalPrice
        }).ToList();

        _logger.LogInformation("Retrieved {Count} orderitems (Page {Page}, PageSize {PageSize})",
            orderItemDtos.Count, request.Page, request.PageSize);

        return new GetOrderItemsResponse
        {
            Data = orderItemDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
