using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Deliveries;

namespace SmartLunch.Backend.Service.Application.Queries.Deliveries.GetDelivery;

public class GetDeliveryQuery : IRequest<GetDeliveryResponse>
{
    public Guid DeliveryId { get; set; }

    public GetDeliveryQuery(Guid deliveryId)
    {
        DeliveryId = deliveryId;
    }
}
