namespace SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Routes;

public class OptimizeRouteRequest
{
    public RoutePointDto Start { get; set; } = new();
    public List<RoutePointDto> Stops { get; set; } = new();
}

public class RoutePointDto
{
    public int? DeliveryId { get; set; }
    public string? Label { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

