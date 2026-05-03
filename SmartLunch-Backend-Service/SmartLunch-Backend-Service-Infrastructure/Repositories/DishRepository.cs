using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class DishRepository : IDishRepository
{
    private readonly SmartLunchDBContext _context;

    public DishRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<List<Dish>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new List<Dish>();

        return await _context.Dishes
            .Where(d => idList.Contains(d.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Dish?> GetByIdAsync(int id)
    {
        return await _context.Dishes
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Dish?> GetByIdWithIngredientsAsync(int id)
    {
        return await _context.Dishes
            .Include(d => d.DishIngredients)
            .ThenInclude(di => di.Ingredient)
            .Include(d => d.DishImages)
            .ThenInclude(img => img.MediaFile)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<(List<Dish> Dishes, int TotalCount)> GetDishesAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null, string? category = null)
    {
        var query = _context.Dishes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(d =>
                d.Name.Contains(searchTerm) ||
                (d.Description != null && d.Description.Contains(searchTerm)) ||
                (d.Category != null && d.Category.Contains(searchTerm)) ||
                (d.DietaryLabel != null && d.DietaryLabel.Contains(searchTerm)));
        }

        if (isActive.HasValue)
            query = query.Where(d => d.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(d => d.Category != null && d.Category == category);

        var totalCount = await query.CountAsync();

        var dishes = await query
            .OrderBy(d => d.Name)
            .Include(d => d.DishImages)
            .ThenInclude(img => img.MediaFile)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (dishes, totalCount);
    }

    public async Task<Dish> CreateAsync(Dish dish)
    {
        _context.Dishes.Add(dish);
        await _context.SaveChangesAsync();
        return dish;
    }

    public async Task<Dish> UpdateAsync(Dish dish)
    {
        _context.Dishes.Update(dish);
        await _context.SaveChangesAsync();
        return dish;
    }
}
