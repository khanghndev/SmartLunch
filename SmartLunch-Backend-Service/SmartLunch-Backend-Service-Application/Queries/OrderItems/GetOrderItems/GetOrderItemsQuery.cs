using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.OrderItems;

namespace SmartLunch.Backend.Service.Application.Queries.OrderItems.GetOrderItems;

public class GetOrderItemsQuery : IRequest<GetOrderItemsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetOrderItemsQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
