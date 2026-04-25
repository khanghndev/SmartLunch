using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDishRepository
{
    Task<List<Dish>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    Task<Dish?> GetByIdAsync(int id);
    Task<Dish?> GetByIdWithIngredientsAsync(int id);
    Task<(List<Dish> Dishes, int TotalCount)> GetDishesAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null, string? category = null);
    Task<Dish> CreateAsync(Dish dish);
    Task<Dish> UpdateAsync(Dish dish);
}
