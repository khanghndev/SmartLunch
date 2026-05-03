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

    public async Task<List<Dish>> GetByIdsWithIngredientsAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new List<Dish>();

        return await _context.Dishes
            .AsNoTracking()
            .Include(d => d.CookingMethod)
            .Include(d => d.DishDishCategories)
                .ThenInclude(ddc => ddc.DishCategory)
            .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
            .Where(d => idList.Contains(d.Id) && d.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Tính popularity (1–5) cho từng DishId dựa trên tổng Quantity đặt
    /// trong <paramref name="lookbackDays"/> ngày gần nhất (không tính đơn cancelled).
    ///
    /// Công thức:
    ///   orderCount[dishId] = SUM(oi.Quantity) trong cửa sổ thời gian
    ///   maxCount           = MAX(orderCount) trong tập dishIds
    ///   popularity         = max(1, min(5, ROUND(orderCount / maxCount * 5)))
    ///   Nếu maxCount = 0   → mọi món = 3 (trung tính)
    /// </summary>
    public async Task<Dictionary<int, int>> GetPopularityScoresAsync(
        IEnumerable<int> dishIds,
        int lookbackDays = 28,
        CancellationToken cancellationToken = default)
    {
        var idList = dishIds.Distinct().ToList();
        var cutoff = DateTime.UtcNow.AddDays(-lookbackDays);

        // Aggregate order quantities per dish within the lookback window
        var counts = await _context.Set<OrderItem>()
            .AsNoTracking()
            .Where(oi =>
                idList.Contains(oi.DishId) &&
                oi.Order.ScheduledDate >= cutoff &&
                oi.Order.Status != "cancelled")
            .GroupBy(oi => oi.DishId)
            .Select(g => new { DishId = g.Key, Total = g.Sum(oi => oi.Quantity) })
            .ToListAsync(cancellationToken);

        var countMap = counts.ToDictionary(c => c.DishId, c => (double)c.Total);
        double maxCount = countMap.Values.Count > 0 ? countMap.Values.Max() : 0;

        var result = new Dictionary<int, int>();
        foreach (var dishId in idList)
        {
            if (maxCount <= 0)
            {
                result[dishId] = 3; // Chưa có dữ liệu → trung tính
            }
            else
            {
                var count = countMap.GetValueOrDefault(dishId, 0);
                var score = (int)Math.Round(count / maxCount * 5);
                result[dishId] = Math.Max(1, Math.Min(5, score));
            }
        }

        return result;
    }

    public async Task<Dish?> GetByIdAsync(int id)
    {
        return await _context.Dishes
            .Include(d => d.CookingMethod)
            .Include(d => d.DishDishCategories)
                .ThenInclude(ddc => ddc.DishCategory)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Dish?> GetByIdWithIngredientsAsync(int id)
    {
        return await _context.Dishes
            .Include(d => d.CookingMethod)
            .Include(d => d.DishDishCategories)
                .ThenInclude(ddc => ddc.DishCategory)
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
            var term = searchTerm.Trim();
            query = query.Where(d =>
                d.Name.Contains(term) ||
                (d.Description != null && d.Description.Contains(term)) ||
                (d.DietaryLabel != null && d.DietaryLabel.Contains(term)) ||
                d.DishDishCategories.Any(ddc =>
                    ddc.DishCategory != null &&
                    (ddc.DishCategory.SlotKey.Contains(term) || ddc.DishCategory.Name.Contains(term))) ||
                (d.CookingMethod != null &&
                    (d.CookingMethod.MethodKey.Contains(term) || d.CookingMethod.Name.Contains(term))));
        }

        if (isActive.HasValue)
            query = query.Where(d => d.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(category))
        {
            var slot = category.Trim();
            query = query.Where(d =>
                d.DishDishCategories.Any(ddc =>
                    ddc.DishCategory != null &&
                    ddc.DishCategory.SlotKey == slot));
        }

        var totalCount = await query.CountAsync();

        var dishes = await query
            .Include(d => d.CookingMethod)
            .Include(d => d.DishDishCategories)
                .ThenInclude(ddc => ddc.DishCategory)
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

    public async Task ReplaceDishDishCategoriesAsync(
        int dishId,
        IReadOnlyList<string> categoryCodes,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.DishDishCategories
            .Where(x => x.DishId == dishId)
            .ToListAsync(cancellationToken);
        _context.DishDishCategories.RemoveRange(existing);

        if (categoryCodes.Count == 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        var normalized = categoryCodes
            .Select(c => c.Trim())
            .Where(c => c.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var catalog = await _context.DishCategories.AsNoTracking().ToListAsync(cancellationToken);

        foreach (var code in normalized)
        {
            var row = catalog.FirstOrDefault(c =>
                string.Equals(c.SlotKey, code, StringComparison.OrdinalIgnoreCase));
            if (row == null)
                throw new ArgumentException($"Unknown dish slot category code: '{code}'.");

            _context.DishDishCategories.Add(new DishDishCategory
            {
                DishId = dishId,
                DishCategoryId = row.Id,
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int?> ResolveCookingMethodIdByMethodKeyAsync(string methodKey, CancellationToken cancellationToken = default)
    {
        var key = methodKey.Trim();
        if (key.Length == 0)
            return null;

        var keyLower = key.ToLowerInvariant();
        return await _context.CookingMethods.AsNoTracking()
            .Where(m => m.MethodKey.ToLower() == keyLower)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
