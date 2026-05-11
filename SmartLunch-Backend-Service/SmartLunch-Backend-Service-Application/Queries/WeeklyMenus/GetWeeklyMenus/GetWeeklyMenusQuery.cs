using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenus;

public class GetWeeklyMenusQuery : IRequest<GetWeeklyMenusResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? CustomerTypeId { get; set; }
    public string? CustomerProfileKey { get; set; }

    public GetWeeklyMenusQuery(int page = 1, int pageSize = 10, string? searchTerm = null, int? customerTypeId = null, string? customerProfileKey = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        CustomerTypeId = customerTypeId;
        CustomerProfileKey = customerProfileKey;
    }
}
