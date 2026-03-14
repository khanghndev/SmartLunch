using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Inventories;

namespace SmartLunch.Backend.Service.Application.Queries.Inventories.GetInventories;

public class GetInventoriesQuery : IRequest<GetInventoriesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetInventoriesQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
