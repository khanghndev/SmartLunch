using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IIngredientRepository
{
    Task<Ingredient?> GetByIdAsync(int id);
    Task<Ingredient?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<Ingredient> Ingredients, int TotalCount)> GetIngredientsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
    Task<List<Ingredient>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<bool> HasBlockingReferencesAsync(int ingredientId, CancellationToken cancellationToken = default);
    Task<Ingredient> CreateAsync(Ingredient ingredient, decimal? reorderLevel = null, CancellationToken cancellationToken = default);
    Task<Ingredient> UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken = default);
    Task DeleteAsync(Ingredient ingredient, CancellationToken cancellationToken = default);
}
