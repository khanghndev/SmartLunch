using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Manager.Deliveries;
using SmartLunch.Backend.Service.Application.DTOs.Response.Manager.Deliveries;

namespace SmartLunch.Backend.Service.Application.Commands.Manager.Deliveries.AssignManagerDelivery;

public record AssignManagerDeliveryCommand(int DeliveryId, AssignManagerDeliveryRequest Request)
    : IRequest<ManagerDeliveryListItemDto>;
