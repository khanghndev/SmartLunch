using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetOrganizationDishesByCategory;

public sealed class GetOrganizationDishesByCategoryQuery : IRequest<GetOrganizationDishesByCategoryResponse>
{
    public GetOrganizationDishesByCategoryQuery(int dishCategoryId, int page, int pageSize)
    {
        DishCategoryId = dishCategoryId;
        Page = page;
        PageSize = pageSize;
    }

    public int DishCategoryId { get; }
    public int Page { get; }
    public int PageSize { get; }
}
