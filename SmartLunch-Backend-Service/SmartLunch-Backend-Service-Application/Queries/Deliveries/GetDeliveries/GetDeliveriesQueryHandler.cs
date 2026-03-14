using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Deliveries;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Deliveries.GetDeliveries;

public class GetDeliveriesQueryHandler : IRequestHandler<GetDeliveriesQuery, GetDeliveriesResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<GetDeliveriesQueryHandler> _logger;

    public GetDeliveriesQueryHandler(IDeliveryRepository deliveryRepository, ILogger<GetDeliveriesQueryHandler> logger)
    {
        _deliveryRepository = deliveryRepository;
        _logger = logger;
    }

    public async Task<GetDeliveriesResponse> Handle(GetDeliveriesQuery request, CancellationToken cancellationToken)
    {
        var (deliveries, totalCount) = await _deliveryRepository.GetDeliveriesAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var deliveryDtos = deliveries.Select(delivery => new DeliveryDto
        {
                Id = delivery.Id,
                OrderId = delivery.OrderId,
                AssignedStaffId = delivery.AssignedStaffId,
                DeliveryAddress = delivery.DeliveryAddress,
                DeliveryStatus = delivery.DeliveryStatus,
                DeliveredAt = delivery.DeliveredAt,
                Notes = delivery.Notes,
                CreatedAt = delivery.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} deliveries (Page {Page}, PageSize {PageSize})",
            deliveryDtos.Count, request.Page, request.PageSize);

        return new GetDeliveriesResponse
        {
            Data = deliveryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
