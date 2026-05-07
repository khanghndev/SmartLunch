using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IWeeklyMenuRepository
{
    Task<WeeklyMenu?> GetByIdAsync(int id);
    Task<WeeklyMenu?> GetByIdWithSchedulesAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<WeeklyMenu> WeeklyMenus, int TotalCount)> GetWeeklyMenusAsync(int page, int pageSize, string? searchTerm = null);
    Task<WeeklyMenu?> GetWeeklyMenuWithSchedulesByDateAsync(DateTime date);
}
