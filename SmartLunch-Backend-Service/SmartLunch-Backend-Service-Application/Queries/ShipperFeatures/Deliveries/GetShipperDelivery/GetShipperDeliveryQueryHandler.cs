using MediatR;
using SmartLunch.Backend.Service.Application.Deliveries;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Deliveries;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.ShipperFeatures.Deliveries.GetShipperDelivery;

public class GetShipperDeliveryQueryHandler : IRequestHandler<GetShipperDeliveryQuery, GetShipperDeliveryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;

    public GetShipperDeliveryQueryHandler(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<GetShipperDeliveryResponse> Handle(GetShipperDeliveryQuery request, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryRepository.GetByIdWithOrderAsync(request.DeliveryId, cancellationToken);
        if (delivery == null)
            throw new KeyNotFoundException($"Delivery with ID {request.DeliveryId} was not found.");

        // Access control: must be assigned to current shipper (or unassigned)
        if (delivery.AssignedStaffId.HasValue && delivery.AssignedStaffId.Value != request.ShipperUserId)
            throw new KeyNotFoundException("Delivery was not found.");

        var mealCount = delivery.Order?.OrderItems?.Sum(i => i.Quantity) ?? 0;

        return new GetShipperDeliveryResponse
        {
            Delivery = new ShipperDeliveryDetailDto
            {
                DeliveryId = delivery.Id,
                OrderId = delivery.OrderId,
                DeliveryAddress = delivery.DeliveryAddress,
                DeliveryStatus = delivery.DeliveryStatus,
                ScheduledDateUtc = delivery.Order?.ScheduledDate ?? DateTime.MinValue,
                MealCount = mealCount,
                DeliveredAtUtc = delivery.DeliveredAt,
                ProofImageUrl = delivery.ProofImageUrl,
                ProofCapturedAtUtc = delivery.ProofCapturedAt,
                Notes = delivery.Notes,
                RecipientConfirmedName = delivery.RecipientConfirmedName,
                RecipientConfirmedAtUtc = delivery.RecipientConfirmedAt,
                RequiresDeliveryOtp = string.Equals(delivery.DeliveryStatus, "in_transit", StringComparison.OrdinalIgnoreCase)
                    && DeliveryOtpService.IsOtpActive(delivery),
            }
        };
    }
}

