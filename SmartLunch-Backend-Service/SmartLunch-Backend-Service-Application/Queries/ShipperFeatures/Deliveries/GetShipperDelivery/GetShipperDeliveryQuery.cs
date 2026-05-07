using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Deliveries;

namespace SmartLunch.Backend.Service.Application.Queries.ShipperFeatures.Deliveries.GetShipperDelivery;

public class GetShipperDeliveryQuery : IRequest<GetShipperDeliveryResponse>
{
    public int ShipperUserId { get; }
    public int DeliveryId { get; }

    public GetShipperDeliveryQuery(int shipperUserId, int deliveryId)
    {
        ShipperUserId = shipperUserId;
        DeliveryId = deliveryId;
    }
}

