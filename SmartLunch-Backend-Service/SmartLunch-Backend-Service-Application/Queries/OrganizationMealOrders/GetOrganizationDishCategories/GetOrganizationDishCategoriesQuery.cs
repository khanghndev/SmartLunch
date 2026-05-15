using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetOrganizationDishCategories;

public sealed class GetOrganizationDishCategoriesQuery : IRequest<GetOrganizationDishCategoriesResponse>
{
}
