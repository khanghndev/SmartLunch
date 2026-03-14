using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IWeeklyMenuRepository
{
    Task<WeeklyMenu?> GetByIdAsync(Guid id);
    Task<(List<WeeklyMenu> WeeklyMenus, int TotalCount)> GetWeeklyMenusAsync(int page, int pageSize, string? searchTerm = null);
}
