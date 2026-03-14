using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class MenuScheduleRepository : IMenuScheduleRepository
{
    private readonly SmartLunchDBContext _context;

    public MenuScheduleRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<MenuSchedule?> GetByIdAsync(Guid id)
    {
        return await _context.MenuSchedules
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<MenuSchedule> MenuSchedules, int TotalCount)> GetMenuSchedulesAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.MenuSchedules.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.MealSlot.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var menuSchedules = await query
            .OrderBy(e => e.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (menuSchedules, totalCount);
    }
}
