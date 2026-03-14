using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetOrder;

public class GetOrderQuery : IRequest<GetOrderResponse>
{
    public Guid OrderId { get; set; }

    public GetOrderQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}
