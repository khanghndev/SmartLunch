using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetOrders;

public class GetOrdersQuery : IRequest<GetOrdersResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetOrdersQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
