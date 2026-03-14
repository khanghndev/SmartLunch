using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Deliveries;

namespace SmartLunch.Backend.Service.Application.Queries.Deliveries.GetDeliveries;

public class GetDeliveriesQuery : IRequest<GetDeliveriesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetDeliveriesQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
