using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Deliveries;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Deliveries.GetDelivery;

public class GetDeliveryQueryHandler : IRequestHandler<GetDeliveryQuery, GetDeliveryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<GetDeliveryQueryHandler> _logger;

    public GetDeliveryQueryHandler(IDeliveryRepository deliveryRepository, ILogger<GetDeliveryQueryHandler> logger)
    {
        _deliveryRepository = deliveryRepository;
        _logger = logger;
    }

    public async Task<GetDeliveryResponse> Handle(GetDeliveryQuery request, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(request.DeliveryId);

        if (delivery == null)
        {
            _logger.LogWarning("Delivery not found with ID: {DeliveryId}", request.DeliveryId);
            return new GetDeliveryResponse { Delivery = new DeliveryDto() };
        }

        return new GetDeliveryResponse
        {
            Delivery = new DeliveryDto
            {
                Id = delivery.Id,
                OrderId = delivery.OrderId,
                AssignedStaffId = delivery.AssignedStaffId,
                DeliveryAddress = delivery.DeliveryAddress,
                DeliveryStatus = delivery.DeliveryStatus,
                DeliveredAt = delivery.DeliveredAt,
                Notes = delivery.Notes,
                CreatedAt = delivery.CreatedAt
            }
        };
    }
}
