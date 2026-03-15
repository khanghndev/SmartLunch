using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.DishIngredients.GetDishIngredients;

public class GetDishIngredientsQueryHandler : IRequestHandler<GetDishIngredientsQuery, GetDishIngredientsResponse>
{
    private readonly IDishIngredientRepository _dishIngredientRepository;

    public GetDishIngredientsQueryHandler(IDishIngredientRepository dishIngredientRepository)
    {
        _dishIngredientRepository = dishIngredientRepository;
    }

    public async Task<GetDishIngredientsResponse> Handle(GetDishIngredientsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _dishIngredientRepository.GetPagedAsync(
            request.Page, request.PageSize, request.DishId, request.IngredientId);

        var dtos = items.Select(di => new DishIngredientDto
        {
            Id = di.Id,
            DishId = di.DishId,
            IngredientId = di.IngredientId,
            DishName = di.Dish?.Name,
            IngredientName = di.Ingredient?.Name,
            Quantity = di.Quantity,
            Unit = di.Unit
        }).ToList();

        return new GetDishIngredientsResponse
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
