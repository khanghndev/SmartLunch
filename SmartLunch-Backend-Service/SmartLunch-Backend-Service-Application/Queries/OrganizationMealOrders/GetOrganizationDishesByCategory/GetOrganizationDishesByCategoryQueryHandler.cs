using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetOrganizationDishesByCategory;

public sealed class GetOrganizationDishesByCategoryQueryHandler
    : IRequestHandler<GetOrganizationDishesByCategoryQuery, GetOrganizationDishesByCategoryResponse>
{
    private readonly IDishRepository _dishRepository;

    public GetOrganizationDishesByCategoryQueryHandler(IDishRepository dishRepository)
    {
        _dishRepository = dishRepository;
    }

    public async Task<GetOrganizationDishesByCategoryResponse> Handle(
        GetOrganizationDishesByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        if (request.DishCategoryId <= 0)
            throw new ArgumentException("categoryId (dish category) is required.");

        var (dishes, total) = await _dishRepository.GetByDishCategoryIdAsync(
            request.DishCategoryId,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new GetOrganizationDishesByCategoryResponse
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total,
            Dishes = dishes.Select(d => new OrganizationDishListItemDto
            {
                Id = d.Id,
                Name = d.Name,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                SlotKeys = d.DishDishCategories
                    .Where(x => x.DishCategory != null)
                    .Select(x => x.DishCategory!.SlotKey)
                    .Distinct()
                    .ToList(),
            }).ToList(),
        };
    }
}
