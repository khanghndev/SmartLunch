using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IMenuScheduleRepository
{
    Task<MenuSchedule?> GetByIdAsync(int id);
    Task<(List<MenuSchedule> MenuSchedules, int TotalCount)> GetMenuSchedulesAsync(int page, int pageSize, string? searchTerm = null);
}
