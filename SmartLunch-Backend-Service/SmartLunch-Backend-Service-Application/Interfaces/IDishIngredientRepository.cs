using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDishIngredientRepository
{
    Task<DishIngredient?> GetByIdAsync(int id);
    Task<DishIngredient?> GetByDishAndIngredientAsync(int dishId, int ingredientId, int dishValueId);
    Task<IEnumerable<DishIngredient>> GetByDishIdAsync(int dishId);
    Task<IEnumerable<DishIngredient>> GetByIngredientIdAsync(int ingredientId);
    Task<(List<DishIngredient> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? dishId = null, int? ingredientId = null);
    Task<DishIngredient> CreateAsync(DishIngredient dishIngredient);
    Task<DishIngredient> UpdateAsync(DishIngredient dishIngredient);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByDishAndIngredientAsync(int dishId, int ingredientId, int dishValueId);
    Task<bool> ExistsByDishAndIngredientAsync(int dishId, int ingredientId, int dishValueId);
}
