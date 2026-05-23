using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Manager.Deliveries;

namespace SmartLunch.Backend.Service.Application.Queries.Manager.Deliveries.GetManagerDeliveries;

public record GetManagerDeliveriesQuery(
    int Page,
    int PageSize,
    string? Status,
    DateOnly? ScheduledOn,
    string? SearchTerm,
    bool? UnassignedOnly) : IRequest<GetManagerDeliveriesResponse>;
