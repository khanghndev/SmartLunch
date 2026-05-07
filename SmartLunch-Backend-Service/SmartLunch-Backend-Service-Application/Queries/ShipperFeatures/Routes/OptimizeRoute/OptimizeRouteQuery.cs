using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Routes;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Routes;

namespace SmartLunch.Backend.Service.Application.Queries.ShipperFeatures.Routes.OptimizeRoute;

public record OptimizeRouteQuery(int ShipperUserId, OptimizeRouteRequest Request) : IRequest<OptimizeRouteResponse>;

