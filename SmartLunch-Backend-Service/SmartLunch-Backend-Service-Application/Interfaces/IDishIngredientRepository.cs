using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDishIngredientRepository
{
    Task<DishIngredient?> GetByIdAsync(Guid id);
    Task<DishIngredient?> GetByDishAndIngredientAsync(Guid dishId, Guid ingredientId);
    Task<IEnumerable<DishIngredient>> GetByDishIdAsync(Guid dishId);
    Task<IEnumerable<DishIngredient>> GetByIngredientIdAsync(Guid ingredientId);
    Task<(List<DishIngredient> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? dishId = null, Guid? ingredientId = null);
    Task<DishIngredient> CreateAsync(DishIngredient dishIngredient);
    Task<DishIngredient> UpdateAsync(DishIngredient dishIngredient);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteByDishAndIngredientAsync(Guid dishId, Guid ingredientId);
    Task<bool> ExistsByDishAndIngredientAsync(Guid dishId, Guid ingredientId);
}
