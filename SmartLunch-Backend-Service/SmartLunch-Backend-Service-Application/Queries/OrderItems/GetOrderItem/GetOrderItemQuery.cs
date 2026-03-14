using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.OrderItems;

namespace SmartLunch.Backend.Service.Application.Queries.OrderItems.GetOrderItem;

public class GetOrderItemQuery : IRequest<GetOrderItemResponse>
{
    public Guid OrderItemId { get; set; }

    public GetOrderItemQuery(Guid orderItemId)
    {
        OrderItemId = orderItemId;
    }
}
