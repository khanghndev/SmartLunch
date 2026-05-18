using MediatR;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

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
        if (req.Price < 0)
            throw new ArgumentException("Price cannot be negative.");

        if (req.DishSlotCategoryCodes is not null)
            DishAiEnglishCatalog.ValidateSlotCategoryListOrThrow(req.DishSlotCategoryCodes);

        // Load with images + ingredients for sync and response
        var entity = await _dishRepository.GetByIdWithIngredientsAsync(request.DishId);
        if (entity == null)
            throw new KeyNotFoundException($"Dish with ID {request.DishId} was not found.");

        entity.Name = req.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
        entity.Price = req.Price;
        entity.DietaryLabel = string.IsNullOrWhiteSpace(req.DietaryLabel) ? null : req.DietaryLabel.Trim();
        entity.Calories = req.Calories;
        entity.Protein = req.Protein;
        entity.Fat = req.Fat;
        entity.Carbs = req.Carbs;
        entity.IsActive = req.IsActive;
        entity.UpdatedAt = VietnamTime.Now;

        if (req.NameEnglish is not null)
            entity.NameEnglish = string.IsNullOrWhiteSpace(req.NameEnglish) ? null : req.NameEnglish.Trim();
        if (req.CookingMethod is not null)
        {
            if (string.IsNullOrWhiteSpace(req.CookingMethod))
            {
                entity.CookingMethodId = null;
            }
            else
            {
                DishAiEnglishCatalog.ValidateCookingMethodOrThrow(req.CookingMethod);
                var mk = req.CookingMethod.Trim();
                entity.CookingMethodId = await _dishRepository.ResolveCookingMethodIdByMethodKeyAsync(mk, cancellationToken)
                    ?? throw new ArgumentException($"Unknown cooking method: '{mk}'.");
            }
        }

        // Synchronize images if provided
        if (req.Images != null)
        {
            if (req.Images.Count > 5)
                throw new ArgumentException("A dish cannot have more than 5 images.");

            // Clear existing and re-add (simpler for this case)
            entity.DishImages.Clear();
            foreach (var imgReq in req.Images)
            {
                entity.DishImages.Add(new DishImage
                {
                    DishId = entity.Id,
                    MediaFileId = imgReq.MediaFileId,
                    Role = string.IsNullOrWhiteSpace(imgReq.Role) ? "gallery" : imgReq.Role.ToLowerInvariant(),
                    SortOrder = imgReq.SortOrder ?? 0
                });
            }

            // Update the primary ImageUrl to match the 'cover' image if possible
            var cover = entity.DishImages
                .OrderByDescending(i => i.Role == "cover")
                .ThenBy(i => i.SortOrder)
                .FirstOrDefault();

            if (cover != null && cover.MediaFile != null)
            {
                entity.ImageUrl = cover.MediaFile.ObjectName;
            }
        }
        else if (!string.IsNullOrWhiteSpace(req.ImageUrl))
        {
            entity.ImageUrl = req.ImageUrl.Trim();
        }

        await _dishRepository.UpdateAsync(entity);

        if (req.DishSlotCategoryCodes is not null)
            await _dishRepository.ReplaceDishDishCategoriesAsync(request.DishId, req.DishSlotCategoryCodes, cancellationToken);

        await _cacheService.RemoveAsync(MasterDataCacheKeys.Dish(request.DishId), cancellationToken);

        // Reload to get fresh data with resolved URLs
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
