using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.SignOrderAnnex;

public record SignOrderAnnexCommand(int OrderId, SignOrderAnnexRequest Request, int UserId)
    : IRequest<GetOrderResponse>;
