using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly SmartLunchDBContext _context;

    public IngredientRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Ingredient?> GetByIdAsync(int id)
    {
        return await _context.Ingredients
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Ingredient?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Ingredients
            .Include(e => e.Category)
            .Include(e => e.DefaultSupplier)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<(List<Ingredient> Ingredients, int TotalCount)> GetIngredientsAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
    {
        var query = _context.Ingredients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(e =>
                e.Name.Contains(term) ||
                e.Unit.Contains(term) ||
                (e.NameEnglish != null && e.NameEnglish.Contains(term)) ||
                (e.Description != null && e.Description.Contains(term)));
        }

        if (isActive.HasValue)
            query = query.Where(e => e.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();

        var ingredients = await query
            .OrderBy(e => e.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (ingredients, totalCount);
    }

    public async Task<List<Ingredient>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Ingredients
            .Include(e => e.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim();
        var query = _context.Ingredients.Where(e => e.Name == normalized);
        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default) =>
        _context.IngredientCategories.AnyAsync(c => c.Id == categoryId, cancellationToken);

    public async Task<bool> HasBlockingReferencesAsync(int ingredientId, CancellationToken cancellationToken = default)
    {
        var hasDishLines = await _context.DishIngredients.AnyAsync(d => d.IngredientId == ingredientId, cancellationToken);
        if (hasDishLines)
            return true;

        return await _context.IngredientActualIntakeLines
            .AnyAsync(l => l.IngredientId == ingredientId, cancellationToken);
    }

    public async Task<Ingredient> CreateAsync(Ingredient ingredient, decimal? reorderLevel = null, CancellationToken cancellationToken = default)
    {
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync(cancellationToken);

        _context.Inventories.Add(new Inventory
        {
            IngredientId = ingredient.Id,
            QuantityAvailable = 0,
            ReorderLevel = reorderLevel,
            LastUpdated = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync(cancellationToken);

        return ingredient;
    }

    public async Task<Ingredient> UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
    {
        _context.Ingredients.Update(ingredient);
        await _context.SaveChangesAsync(cancellationToken);
        return ingredient;
    }

    public async Task DeleteAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
    {
        _context.Ingredients.Remove(ingredient);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
