namespace SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Deliveries;

public class GetShipperDeliveriesResponse
{
    public List<ShipperDeliveryListItemDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

