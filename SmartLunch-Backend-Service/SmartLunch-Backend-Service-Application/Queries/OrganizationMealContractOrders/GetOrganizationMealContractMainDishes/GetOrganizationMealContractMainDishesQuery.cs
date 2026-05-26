using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealContractOrders.GetOrganizationMealContractMainDishes;

public sealed class GetOrganizationMealContractMainDishesQuery : IRequest<GetOrganizationDishesByCategoryResponse>
{
    public GetOrganizationMealContractMainDishesQuery(int page, int pageSize, string? search = null)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
    }

    public int Page { get; }
    public int PageSize { get; }
    public string? Search { get; }
}
