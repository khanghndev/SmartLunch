using MediatR;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.DishIngredients.DeleteDishIngredient;

public class DeleteDishIngredientCommandHandler : IRequestHandler<DeleteDishIngredientCommand, bool>
{
    private readonly IDishIngredientRepository _dishIngredientRepository;
    private readonly ICacheService _cacheService;

    public DeleteDishIngredientCommandHandler(
        IDishIngredientRepository dishIngredientRepository,
        ICacheService cacheService)
    {
        _dishIngredientRepository = dishIngredientRepository;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeleteDishIngredientCommand request, CancellationToken cancellationToken)
    {
        var existing = await _dishIngredientRepository.GetByIdAsync(request.Id);
        if (existing == null)
            throw new KeyNotFoundException($"DishIngredient with ID {request.Id} not found");

        var dishId = existing.DishId;
        var deleted = await _dishIngredientRepository.DeleteAsync(request.Id);
        if (!deleted)
            throw new KeyNotFoundException($"DishIngredient with ID {request.Id} not found");

        await _cacheService.RemoveAsync(MasterDataCacheKeys.Dish(dishId), cancellationToken);
        return true;
    }
}
