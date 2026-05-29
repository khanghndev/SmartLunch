using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.CancelCustomerOrder;

public class CancelCustomerOrderCommand : IRequest<GetOrderResponse>
{
    public int OrderId { get; }
    public int UserId { get; }

    public CancelCustomerOrderCommand(int orderId, int userId)
    {
        OrderId = orderId;
        UserId = userId;
    }
}
