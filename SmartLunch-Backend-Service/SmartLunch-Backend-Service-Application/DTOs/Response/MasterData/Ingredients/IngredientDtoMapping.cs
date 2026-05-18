using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;

public static class IngredientDtoMapping
{
    public static IngredientDto ToDto(Ingredient ingredient) => new()
    {
        Id = ingredient.Id,
        Name = ingredient.Name,
        NameEnglish = ingredient.NameEnglish,
        Unit = ingredient.Unit,
        Description = ingredient.Description,
        DefaultSupplierId = ingredient.DefaultSupplierId,
        CostPerUnit = ingredient.CostPerUnit,
        CategoryId = ingredient.CategoryId,
        IsActive = ingredient.IsActive,
        CreatedAt = ingredient.CreatedAt,
        UpdatedAt = ingredient.UpdatedAt,
    };
}
