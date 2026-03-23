using MediatR;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Dishes.UpdateDish;

public class UpdateDishCommandHandler : IRequestHandler<UpdateDishCommand, GetDishResponse>
{
    private readonly IDishRepository _dishRepository;
    private readonly ICacheService _cacheService;

    public UpdateDishCommandHandler(IDishRepository dishRepository, ICacheService cacheService)
    {
        _dishRepository = dishRepository;
        _cacheService = cacheService;
    }

    public async Task<GetDishResponse> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new ArgumentException("Name is required.");
        if (!DishCatalogCategory.IsValidOrEmpty(req.Category))
            throw new ArgumentException(
                "Invalid category. Use one of: man, xao, canh, trang_mieng, or leave empty.");
        if (req.Price < 0)
            throw new ArgumentException("Price cannot be negative.");

        var entity = await _dishRepository.GetByIdAsync(request.DishId);
        if (entity == null)
            throw new KeyNotFoundException($"Dish with ID {request.DishId} was not found.");

        entity.Name = req.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
        entity.Category = DishCatalogCategory.Normalize(req.Category);
        entity.Price = req.Price;
        entity.DietaryLabel = string.IsNullOrWhiteSpace(req.DietaryLabel) ? null : req.DietaryLabel.Trim();
        entity.IsActive = req.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dishRepository.UpdateAsync(entity);
        await _cacheService.RemoveAsync(MasterDataCacheKeys.Dish(request.DishId), cancellationToken);

        var reloaded = await _dishRepository.GetByIdWithIngredientsAsync(entity.Id);
        var dish = reloaded ?? entity;
        return new GetDishResponse
        {
            Dish = DishDtoMapping.ToDto(dish),
            IngredientQuotas = reloaded?.DishIngredients.OrderBy(di => di.Ingredient?.Name).Select(DishDtoMapping.ToQuotaDto).ToList()
                ?? new List<DishIngredientQuotaDto>()
        };
    }
}
