namespace SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Routes;

public class OptimizeRouteResponse
{
    public List<OptimizedStopDto> Stops { get; set; } = new();
    public double ApproxTotalDistanceKm { get; set; }
}

public class OptimizedStopDto
{
    public int Sequence { get; set; }
    public int? DeliveryId { get; set; }
    public string? Label { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

