using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Manager.Deliveries;

namespace SmartLunch.Backend.Service.Application.Queries.Manager.Deliveries.GetManagerShippers;

public record GetManagerShippersQuery : IRequest<GetManagerShippersResponse>;
