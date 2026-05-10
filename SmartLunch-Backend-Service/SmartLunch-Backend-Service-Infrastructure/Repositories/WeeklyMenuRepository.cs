using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class WeeklyMenuRepository : IWeeklyMenuRepository
{
    private readonly SmartLunchDBContext _context;

    public WeeklyMenuRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<WeeklyMenu?> GetByIdAsync(int id)
    {
        return await _context.WeeklyMenus
            .Include(m => m.WeeklyMenuImages)
            .ThenInclude(img => img.MediaFile)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<WeeklyMenu?> GetByIdWithSchedulesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.WeeklyMenus
            .Include(wm => wm.MenuSchedules)
                .ThenInclude(ms => ms.Dish)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<WeeklyMenu?> GetByIdWithSchedulesAndImagesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.WeeklyMenus
            .Include(wm => wm.WeeklyMenuImages)
                .ThenInclude(img => img.MediaFile)
            .Include(wm => wm.MenuSchedules)
                .ThenInclude(ms => ms.Dish)
                    .ThenInclude(d => d.DishDishCategories)
                        .ThenInclude(ddc => ddc.DishCategory)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<(List<WeeklyMenu> WeeklyMenus, int TotalCount)> GetWeeklyMenusAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? customerTypeId = null)
    {
        var query = _context.WeeklyMenus.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                (e.Description != null && e.Description.Contains(searchTerm)));
        }

        if (customerTypeId.HasValue)
            query = query.Where(e => e.CustomerTypeId == customerTypeId.Value);

        var totalCount = await query.CountAsync();

        var weeklyMenus = await query
            .OrderBy(e => e.StartDate)
            .Include(m => m.WeeklyMenuImages)
            .ThenInclude(img => img.MediaFile)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (weeklyMenus, totalCount);
    }

    public async Task<WeeklyMenu?> GetWeeklyMenuWithSchedulesByDateAsync(DateTime date, int? customerTypeId = null)
    {
        var query = _context.WeeklyMenus
            .Include(wm => wm.MenuSchedules)
                .ThenInclude(ms => ms.Dish)
                    .ThenInclude(d => d.DishDishCategories)
                        .ThenInclude(ddc => ddc.DishCategory)
            .Where(wm => wm.StartDate <= date && wm.EndDate >= date);

        if (customerTypeId.HasValue)
            query = query.Where(wm => wm.CustomerTypeId == customerTypeId.Value);

        return await query
            .OrderByDescending(wm => wm.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
