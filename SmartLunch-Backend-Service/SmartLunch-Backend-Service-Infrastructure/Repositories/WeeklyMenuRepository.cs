using System.Globalization;
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
                    .ThenInclude(d => d.DishImages)
                        .ThenInclude(di => di.MediaFile)
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
        int? customerTypeId = null,
        DateTime? effectiveDate = null,
        DateTime? notEndedBefore = null)
    {
        var query = _context.WeeklyMenus.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            if (int.TryParse(term, NumberStyles.Integer, CultureInfo.InvariantCulture, out var idMatch))
            {
                query = query.Where(e =>
                    e.Id == idMatch
                    || (e.Description != null && e.Description.Contains(term)));
            }
            else
            {
                query = query.Where(e =>
                    (e.Description != null && e.Description.Contains(term)));
            }
        }

        if (customerTypeId.HasValue)
            query = query.Where(e => e.CustomerTypeId == customerTypeId.Value);

        if (effectiveDate.HasValue)
        {
            var d = effectiveDate.Value;
            query = query.Where(wm => wm.StartDate <= d && wm.EndDate >= d);
        }

        if (notEndedBefore.HasValue)
        {
            var d = notEndedBefore.Value.Date;
            query = query.Where(wm => wm.EndDate.Date >= d);
        }

        var totalCount = await query.CountAsync();

        var weeklyMenus = await query
            .OrderByDescending(e => e.StartDate)
            .ThenByDescending(e => e.Id)
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

    public async Task<bool> ExistsByPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        string menuType,
        int? customerTypeId,
        CancellationToken cancellationToken = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;
        var type = menuType.Trim();

        return await _context.WeeklyMenus.AnyAsync(
            wm => wm.StartDate.Date == start
                  && wm.EndDate.Date == end
                  && wm.MenuType == type
                  && wm.CustomerTypeId == customerTypeId,
            cancellationToken);
    }

    public async Task<WeeklyMenu> CreateWithSchedulesAsync(
        WeeklyMenu menu,
        IReadOnlyList<MenuSchedule> schedules,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _context.WeeklyMenus.Add(menu);
            await _context.SaveChangesAsync(cancellationToken);

            foreach (var schedule in schedules)
                schedule.MenuId = menu.Id;

            if (schedules.Count > 0)
            {
                _context.MenuSchedules.AddRange(schedules);
                await _context.SaveChangesAsync(cancellationToken);
            }

            await tx.CommitAsync(cancellationToken);
            return menu;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
