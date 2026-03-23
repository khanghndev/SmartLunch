using MediatR;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.DishIngredients;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.CreateDishIngredient;

public class CreateDishIngredientCommandHandler : IRequestHandler<CreateDishIngredientCommand, CreateDishIngredientResponse>
{
    private readonly IDishIngredientRepository _dishIngredientRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ICacheService _cacheService;

    public CreateDishIngredientCommandHandler(
        IDishIngredientRepository dishIngredientRepository,
        IDishRepository dishRepository,
        IIngredientRepository ingredientRepository,
        ICacheService cacheService)
    {
        _dishIngredientRepository = dishIngredientRepository;
        _dishRepository = dishRepository;
        _ingredientRepository = ingredientRepository;
        _cacheService = cacheService;
    }

    public async Task<CreateDishIngredientResponse> Handle(CreateDishIngredientCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (await _dishRepository.GetByIdAsync(req.DishId) == null)
            throw new KeyNotFoundException($"Dish with ID {req.DishId} not found");
        if (await _ingredientRepository.GetByIdAsync(req.IngredientId) == null)
            throw new KeyNotFoundException($"Ingredient with ID {req.IngredientId} not found");
        if (await _dishIngredientRepository.ExistsByDishAndIngredientAsync(req.DishId, req.IngredientId))
            throw new InvalidOperationException("Dish already has this ingredient");

        var entity = new DishIngredient
        {
            Id = Guid.NewGuid(),
            DishId = req.DishId,
            IngredientId = req.IngredientId,
            Quantity = req.Quantity,
            Unit = req.Unit
        };
        await _dishIngredientRepository.CreateAsync(entity);
        await _cacheService.RemoveAsync(MasterDataCacheKeys.Dish(req.DishId), cancellationToken);

        return new CreateDishIngredientResponse
        {
            Id = entity.Id,
            DishId = entity.DishId,
            IngredientId = entity.IngredientId,
            Message = "Dish ingredient created successfully"
        };
    }
}
