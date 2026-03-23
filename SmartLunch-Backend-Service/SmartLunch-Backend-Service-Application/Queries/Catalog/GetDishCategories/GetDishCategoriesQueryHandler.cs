using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.Catalog;

namespace SmartLunch.Backend.Service.Application.Queries.Catalog.GetDishCategories;

public class GetDishCategoriesQueryHandler : IRequestHandler<GetDishCategoriesQuery, GetDishCategoriesResponse>
{
    public Task<GetDishCategoriesResponse> Handle(GetDishCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = DishCatalogCategory.All
            .Select(x => new DishCategoryOptionDto { Code = x.Code, DisplayName = x.DisplayName })
            .ToList();
        return Task.FromResult(new GetDishCategoriesResponse { Categories = categories });
    }
}
