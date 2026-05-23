using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetPublicDishesBrowse;

public sealed class GetPublicDishesBrowseQuery : IRequest<GetPublicDishesBrowseResponse>
{
    public GetPublicDishesBrowseQuery(int page, int pageSize, string? search, int? categoryId)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
        CategoryId = categoryId;
    }

    public int Page { get; }
    public int PageSize { get; }
    public string? Search { get; }
    public int? CategoryId { get; }
}
