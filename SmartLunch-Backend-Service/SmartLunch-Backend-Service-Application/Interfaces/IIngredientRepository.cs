using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IIngredientRepository
{
    Task<Ingredient?> GetByIdAsync(Guid id);
    Task<(List<Ingredient> Ingredients, int TotalCount)> GetIngredientsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
}
