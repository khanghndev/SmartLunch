using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Routes;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Routes;

namespace SmartLunch.Backend.Service.Application.Queries.ShipperFeatures.Routes.OptimizeRoute;

public class OptimizeRouteQueryHandler : IRequestHandler<OptimizeRouteQuery, OptimizeRouteResponse>
{
    public Task<OptimizeRouteResponse> Handle(OptimizeRouteQuery request, CancellationToken cancellationToken)
    {
        var start = request.Request.Start;
        var stops = request.Request.Stops ?? new List<RoutePointDto>();
        if (stops.Count == 0)
            return Task.FromResult(new OptimizeRouteResponse());

        var remaining = new List<RoutePointDto>(stops);
        var currentLat = start.Latitude;
        var currentLng = start.Longitude;

        var result = new OptimizeRouteResponse();
        var seq = 1;
        double totalKm = 0;

        while (remaining.Count > 0)
        {
            var bestIdx = 0;
            var bestDist = double.MaxValue;

            for (var i = 0; i < remaining.Count; i++)
            {
                var d = HaversineKm(currentLat, currentLng, remaining[i].Latitude, remaining[i].Longitude);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestIdx = i;
                }
            }

            var next = remaining[bestIdx];
            remaining.RemoveAt(bestIdx);

            totalKm += bestDist;
            currentLat = next.Latitude;
            currentLng = next.Longitude;

            result.Stops.Add(new OptimizedStopDto
            {
                Sequence = seq++,
                DeliveryId = next.DeliveryId,
                Label = next.Label,
                Latitude = next.Latitude,
                Longitude = next.Longitude
            });
        }

        result.ApproxTotalDistanceKm = Math.Round(totalKm, 3);
        return Task.FromResult(result);
    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}

