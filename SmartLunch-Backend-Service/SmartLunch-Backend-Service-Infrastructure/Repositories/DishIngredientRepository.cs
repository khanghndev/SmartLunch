using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class DishIngredientRepository : IDishIngredientRepository
{
    private readonly SmartLunchDBContext _context;

    public DishIngredientRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<DishIngredient?> GetByIdAsync(Guid id)
    {
        return await _context.DishIngredients
            .Include(di => di.Dish)
            .Include(di => di.Ingredient)
            .FirstOrDefaultAsync(di => di.Id == id);
    }

    public async Task<DishIngredient?> GetByDishAndIngredientAsync(Guid dishId, Guid ingredientId)
    {
        return await _context.DishIngredients
            .Include(di => di.Dish)
            .Include(di => di.Ingredient)
            .FirstOrDefaultAsync(di => di.DishId == dishId && di.IngredientId == ingredientId);
    }

    public async Task<IEnumerable<DishIngredient>> GetByDishIdAsync(Guid dishId)
    {
        return await _context.DishIngredients
            .Include(di => di.Dish)
            .Include(di => di.Ingredient)
            .Where(di => di.DishId == dishId)
            .ToListAsync();
    }

    public async Task<IEnumerable<DishIngredient>> GetByIngredientIdAsync(Guid ingredientId)
    {
        return await _context.DishIngredients
            .Include(di => di.Dish)
            .Include(di => di.Ingredient)
            .Where(di => di.IngredientId == ingredientId)
            .ToListAsync();
    }

    public async Task<(List<DishIngredient> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? dishId = null, Guid? ingredientId = null)
    {
        var query = _context.DishIngredients
            .Include(di => di.Dish)
            .Include(di => di.Ingredient)
            .AsQueryable();

        if (dishId.HasValue)
            query = query.Where(di => di.DishId == dishId.Value);
        if (ingredientId.HasValue)
            query = query.Where(di => di.IngredientId == ingredientId.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(di => di.DishId)
            .ThenBy(di => di.IngredientId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<DishIngredient> CreateAsync(DishIngredient dishIngredient)
    {
        _context.DishIngredients.Add(dishIngredient);
        await _context.SaveChangesAsync();
        return dishIngredient;
    }

    public async Task<DishIngredient> UpdateAsync(DishIngredient dishIngredient)
    {
        _context.DishIngredients.Update(dishIngredient);
        await _context.SaveChangesAsync();
        return dishIngredient;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var dishIngredient = await _context.DishIngredients.FindAsync(id);
        if (dishIngredient == null) return false;

        _context.DishIngredients.Remove(dishIngredient);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByDishAndIngredientAsync(Guid dishId, Guid ingredientId)
    {
        var dishIngredient = await GetByDishAndIngredientAsync(dishId, ingredientId);
        if (dishIngredient == null) return false;

        _context.DishIngredients.Remove(dishIngredient);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByDishAndIngredientAsync(Guid dishId, Guid ingredientId)
    {
        return await _context.DishIngredients.AnyAsync(di => di.DishId == dishId && di.IngredientId == ingredientId);
    }
}
