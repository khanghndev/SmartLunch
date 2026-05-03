using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

public static class DishDtoMapping
{
    public static DishDto ToDto(Dish d) => new()
    {
        Id = d.Id,
        Code = d.Code,
        Name = d.Name,
        Description = d.Description,
        Category = d.Category,
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
            Url = string.Empty // Placeholder, usually resolved by the caller or a post-process
        }).ToList() ?? new()
    };

    public static DishIngredientQuotaDto ToQuotaDto(DishIngredient di) => new()
    {
        Id = di.Id,
        IngredientId = di.IngredientId,
        IngredientName = di.Ingredient?.Name ?? string.Empty,
        Quantity = di.Quantity,
        Unit = di.Unit
    };
}
