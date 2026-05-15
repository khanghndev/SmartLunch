using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetOrganizationDishCategories;

public sealed class GetOrganizationDishCategoriesQueryHandler
    : IRequestHandler<GetOrganizationDishCategoriesQuery, GetOrganizationDishCategoriesResponse>
{
    private readonly IDishCategoryRepository _dishCategoryRepository;

    public GetOrganizationDishCategoriesQueryHandler(IDishCategoryRepository dishCategoryRepository)
    {
        _dishCategoryRepository = dishCategoryRepository;
    }

    public async Task<GetOrganizationDishCategoriesResponse> Handle(
        GetOrganizationDishCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _dishCategoryRepository.GetAllOrderedAsync(cancellationToken);
        //var (first, last) = OrganizationMealOrderDateWindow.GetAllowedServiceDateRange(DateTime.UtcNow);

        return new GetOrganizationDishCategoriesResponse
        {
            // AllowedFirstServiceDate = first,
            // AllowedLastServiceDate = last,
            Categories = rows
                .Select(c => new OrganizationDishCategoryListItemDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    SlotKey = c.SlotKey,
                    Name = c.Name,
                    SortOrder = c.SortOrder,
                })
                .ToList(),
        };
    }
}
