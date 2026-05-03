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

    public async Task<(List<WeeklyMenu> WeeklyMenus, int TotalCount)> GetWeeklyMenusAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.WeeklyMenus.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                (e.Description != null && e.Description.Contains(searchTerm)));
        }

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
}
