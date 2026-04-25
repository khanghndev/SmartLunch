using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.OrderItems;

namespace SmartLunch.Backend.Service.Application.Queries.OrderItems.GetOrderItem;

public class GetOrderItemQuery : IRequest<GetOrderItemResponse>
{
    public int OrderItemId { get; set; }

    public GetOrderItemQuery(int orderItemId)
    {
        OrderItemId = orderItemId;
    }
}
