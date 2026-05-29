using System;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IWeeklyMenuRepository
{
    Task<WeeklyMenu?> GetByIdAsync(int id);
    Task<WeeklyMenu?> GetByIdWithSchedulesAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Weekly menu với ảnh (cover/gallery) và toàn bộ <see cref="MenuSchedule"/> kèm <see cref="Dish"/>.
    /// </summary>
    Task<WeeklyMenu?> GetByIdWithSchedulesAndImagesAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<WeeklyMenu> WeeklyMenus, int TotalCount)> GetWeeklyMenusAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        int? customerTypeId = null,
        DateTime? effectiveDate = null,
        DateTime? notEndedBefore = null);

    Task<WeeklyMenu?> GetWeeklyMenuWithSchedulesByDateAsync(DateTime date, int? customerTypeId = null);

    Task<bool> ExistsByPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        string menuType,
        int? customerTypeId,
        CancellationToken cancellationToken = default);

    Task<WeeklyMenu> CreateWithSchedulesAsync(
        WeeklyMenu menu,
        IReadOnlyList<MenuSchedule> schedules,
        CancellationToken cancellationToken = default);
}
