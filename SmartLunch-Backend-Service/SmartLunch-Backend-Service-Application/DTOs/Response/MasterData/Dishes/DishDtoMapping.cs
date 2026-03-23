using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

public static class DishDtoMapping
{
    public static DishDto ToDto(Dish d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Description = d.Description,
        Category = d.Category,
        Price = d.Price,
        DietaryLabel = d.DietaryLabel,
        IsActive = d.IsActive,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt
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
