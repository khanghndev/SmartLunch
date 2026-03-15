using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.DishIngredients;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.UpdateDishIngredient;

public class UpdateDishIngredientCommandHandler : IRequestHandler<UpdateDishIngredientCommand, GetDishIngredientResponse>
{
    private readonly IDishIngredientRepository _dishIngredientRepository;

    public UpdateDishIngredientCommandHandler(IDishIngredientRepository dishIngredientRepository)
    {
        _dishIngredientRepository = dishIngredientRepository;
    }

    public async Task<GetDishIngredientResponse> Handle(UpdateDishIngredientCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var entity = await _dishIngredientRepository.GetByIdAsync(req.Id);
        if (entity == null)
            throw new KeyNotFoundException($"DishIngredient with ID {req.Id} not found");

        entity.Quantity = req.Quantity;
        entity.Unit = req.Unit;
        await _dishIngredientRepository.UpdateAsync(entity);

        return new GetDishIngredientResponse
        {
            DishIngredient = new DishIngredientDto
            {
                Id = entity.Id,
                DishId = entity.DishId,
                IngredientId = entity.IngredientId,
                DishName = entity.Dish?.Name,
                IngredientName = entity.Ingredient?.Name,
                Quantity = entity.Quantity,
                Unit = entity.Unit
            }
        };
    }
}
