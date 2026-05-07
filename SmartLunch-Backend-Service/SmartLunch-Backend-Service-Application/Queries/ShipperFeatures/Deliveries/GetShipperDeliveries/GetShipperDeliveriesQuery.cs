using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Deliveries;

namespace SmartLunch.Backend.Service.Application.Queries.ShipperFeatures.Deliveries.GetShipperDeliveries;

public class GetShipperDeliveriesQuery : IRequest<GetShipperDeliveriesResponse>
{
    public int ShipperUserId { get; }
    public int Page { get; }
    public int PageSize { get; }
    public string? Status { get; }
    public DateOnly? ScheduledOn { get; }

    public GetShipperDeliveriesQuery(
        int shipperUserId,
        int page = 1,
        int pageSize = 10,
        string? status = null,
        DateOnly? scheduledOn = null)
    {
        ShipperUserId = shipperUserId;
        Page = page;
        PageSize = pageSize;
        Status = status;
        ScheduledOn = scheduledOn;
    }
}

