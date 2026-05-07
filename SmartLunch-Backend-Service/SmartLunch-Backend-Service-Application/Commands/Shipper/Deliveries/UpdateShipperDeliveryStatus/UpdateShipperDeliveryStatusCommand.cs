using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Deliveries;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Deliveries;

namespace SmartLunch.Backend.Service.Application.Commands.Shipper.Deliveries.UpdateShipperDeliveryStatus;

public record UpdateShipperDeliveryStatusCommand(
    int ShipperUserId,
    int DeliveryId,
    UpdateShipperDeliveryStatusRequest Request) : IRequest<GetShipperDeliveryResponse>;

