using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDishRepository
{
    Task<Dish?> GetByIdAsync(Guid id);
    Task<(List<Dish> Dishes, int TotalCount)> GetDishesAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null, string? category = null);
}
