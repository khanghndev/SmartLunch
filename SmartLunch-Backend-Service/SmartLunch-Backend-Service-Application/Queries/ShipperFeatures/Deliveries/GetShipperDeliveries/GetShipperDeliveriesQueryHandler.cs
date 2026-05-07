using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Deliveries;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.ShipperFeatures.Deliveries.GetShipperDeliveries;

public class GetShipperDeliveriesQueryHandler : IRequestHandler<GetShipperDeliveriesQuery, GetShipperDeliveriesResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<GetShipperDeliveriesQueryHandler> _logger;

    public GetShipperDeliveriesQueryHandler(
        IDeliveryRepository deliveryRepository,
        ILogger<GetShipperDeliveriesQueryHandler> logger)
    {
        _deliveryRepository = deliveryRepository;
        _logger = logger;
    }

    public async Task<GetShipperDeliveriesResponse> Handle(GetShipperDeliveriesQuery request, CancellationToken cancellationToken)
    {
        var (deliveries, totalCount) = await _deliveryRepository.GetForShipperAsync(
            request.ShipperUserId,
            request.Page,
            request.PageSize,
            request.Status,
            request.ScheduledOn,
            cancellationToken);

        var data = deliveries.Select(d =>
        {
            var mealCount = d.Order?.OrderItems?.Sum(i => i.Quantity) ?? 0;
            return new ShipperDeliveryListItemDto
            {
                DeliveryId = d.Id,
                OrderId = d.OrderId,
                DeliveryAddress = d.DeliveryAddress,
                DeliveryStatus = d.DeliveryStatus,
                ScheduledDateUtc = d.Order?.ScheduledDate ?? DateTime.MinValue,
                MealCount = mealCount
            };
        }).ToList();

        _logger.LogInformation("Retrieved {Count} shipper deliveries for user {UserId}", data.Count, request.ShipperUserId);

        return new GetShipperDeliveriesResponse
        {
            Data = data,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}

