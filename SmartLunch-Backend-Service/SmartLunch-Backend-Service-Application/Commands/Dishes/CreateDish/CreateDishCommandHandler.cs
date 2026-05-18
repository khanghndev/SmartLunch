using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Dishes.CreateDish;

public class CreateDishCommandHandler : IRequestHandler<CreateDishCommand, GetDishResponse>
{
    private readonly IDishRepository _dishRepository;

    public CreateDishCommandHandler(IDishRepository dishRepository)
    {
        _dishRepository = dishRepository;
    }

    public async Task<GetDishResponse> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new ArgumentException("Name is required.");
        if (req.Price < 0)
            throw new ArgumentException("Price cannot be negative.");
        if (req.DishSlotCategoryCodes is not { Count: > 0 })
            throw new ArgumentException("DishSlotCategoryCodes is required and must contain at least one slot key (e.g. main, soup).");

        DishAiEnglishCatalog.ValidateSlotCategoryListOrThrow(req.DishSlotCategoryCodes);
        if (string.IsNullOrWhiteSpace(req.CookingMethod))
            throw new ArgumentException("CookingMethod is required (cooking_methods.MethodKey, e.g. fried, stewed).");
        DishAiEnglishCatalog.ValidateCookingMethodOrThrow(req.CookingMethod);

        var methodKey = req.CookingMethod.Trim();
        var cookingMethodId = await _dishRepository.ResolveCookingMethodIdByMethodKeyAsync(methodKey, cancellationToken)
            ?? throw new ArgumentException($"Unknown cooking method: '{methodKey}'.");

        var entity = new Dish
        {
            Name = req.Name.Trim(),
            NameEnglish = string.IsNullOrWhiteSpace(req.NameEnglish) ? null : req.NameEnglish.Trim(),
            Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
            CookingMethodId = cookingMethodId,
            Price = req.Price,
            DietaryLabel = string.IsNullOrWhiteSpace(req.DietaryLabel) ? null : req.DietaryLabel.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(req.ImageUrl) ? null : req.ImageUrl.Trim(),
            Calories = req.Calories,
            Protein = req.Protein,
            Fat = req.Fat,
            Carbs = req.Carbs,
            IsActive = req.IsActive,
            CreatedAt = VietnamTime.Now
        };

        if (req.Images != null && req.Images.Count > 0)
        {
            if (req.Images.Count > 5)
                throw new ArgumentException("A dish cannot have more than 5 images.");

            foreach (var imgReq in req.Images)
            {
                entity.DishImages.Add(new DishImage
                {
                    MediaFileId = imgReq.MediaFileId,
                    Role = string.IsNullOrWhiteSpace(imgReq.Role) ? "gallery" : imgReq.Role.ToLowerInvariant(),
                    SortOrder = imgReq.SortOrder ?? 0
                });
            }

            // Temporarily use objectName if we can (though MediaFile isn't loaded yet, 
            // the repository will save it and we reload later)
            // But actually we can't get ObjectName here without querying MediaFiles.
            // We'll let the reload handle it.
        }

        await _dishRepository.CreateAsync(entity);
        await _dishRepository.ReplaceDishDishCategoriesAsync(entity.Id, req.DishSlotCategoryCodes, cancellationToken);

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
