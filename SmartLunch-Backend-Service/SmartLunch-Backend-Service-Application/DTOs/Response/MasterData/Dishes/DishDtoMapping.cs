using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

public static class DishDtoMapping
{
    public static DishDto ToDto(Dish d)
    {
        var slots = DishAiEnglishCatalog.BuildSlotKeysFromJunction(d);
        return new DishDto
        {
            Id = d.Id,
            Code = d.Code,
            Name = d.Name,
            NameEnglish = d.NameEnglish,
            Description = d.Description,
            PrimarySlotKey = slots.Count > 0 ? DishAiEnglishCatalog.PickPrimaryCategoryForAi(slots) : null,
            DishSlotCategoryCodes = slots,
            CookingMethod = d.CookingMethod?.MethodKey,
            Price = d.Price,
            DietaryLabel = d.DietaryLabel,
            ImageUrl = d.DishImages != null && d.DishImages.Count > 0
                ? (d.DishImages
                    .OrderByDescending(i => string.Equals(i.Role, "cover", StringComparison.OrdinalIgnoreCase))
                    .ThenBy(i => i.SortOrder)
                    .Select(i => i.MediaFile != null ? i.MediaFile.ObjectName : null)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? d.ImageUrl)
                : d.ImageUrl,
            Calories = d.Calories,
            Protein = d.Protein,
            Fat = d.Fat,
            Carbs = d.Carbs,
            IsActive = d.IsActive,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            Images = d.DishImages?.Select(i => new DishImageDto
            {
                Id = i.Id,
                MediaFileId = i.MediaFileId,
                Role = i.Role,
                SortOrder = i.SortOrder,
                Url = string.Empty // Placeholder — caller hoặc bước sau gắn URL đầy đủ
            }).ToList() ?? new List<DishImageDto>()
        };
    }

    /// <summary>Chuỗi hiển thị nhóm món từ tên danh mục slot (dish_categories.Name).</summary>
    public static string? FormatMealSlotNamesDisplay(Dish d)
    {
        var parts = (d.DishDishCategories ?? Enumerable.Empty<DishDishCategory>())
            .Where(x => x.DishCategory != null)
            .OrderBy(x => x.DishCategory!.SortOrder)
            .Select(x => x.DishCategory!.Name)
            .Distinct()
            .ToList();
        return parts.Count == 0 ? null : string.Join(" · ", parts);
    }

    public static DishIngredientQuotaDto ToQuotaDto(DishIngredient di) => new()
    {
        Id = di.Id,
        IngredientId = di.IngredientId,
        IngredientName = di.Ingredient?.Name ?? string.Empty,
        Quantity = di.Quantity,
        Unit = di.Unit
    };
}
