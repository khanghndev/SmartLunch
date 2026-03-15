using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.DishIngredients.GetDishIngredient;

public class GetDishIngredientQueryHandler : IRequestHandler<GetDishIngredientQuery, GetDishIngredientResponse>
{
    private readonly IDishIngredientRepository _dishIngredientRepository;

    public GetDishIngredientQueryHandler(IDishIngredientRepository dishIngredientRepository)
    {
        _dishIngredientRepository = dishIngredientRepository;
    }

    public async Task<GetDishIngredientResponse> Handle(GetDishIngredientQuery request, CancellationToken cancellationToken)
    {
        var di = await _dishIngredientRepository.GetByIdAsync(request.Id);
        if (di == null)
            return new GetDishIngredientResponse { DishIngredient = new DishIngredientDto() };

        return new GetDishIngredientResponse
        {
            DishIngredient = new DishIngredientDto
            {
                Id = di.Id,
                DishId = di.DishId,
                IngredientId = di.IngredientId,
                DishName = di.Dish?.Name,
                IngredientName = di.Ingredient?.Name,
                Quantity = di.Quantity,
                Unit = di.Unit
            }
        };
    }
}
