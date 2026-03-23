using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest<GetOrderResponse>
{
    public Guid OrderId { get; }
    public UpdateOrderStatusRequest Request { get; }

    public UpdateOrderStatusCommand(Guid orderId, UpdateOrderStatusRequest request)
    {
        OrderId = orderId;
        Request = request;
    }
}
