using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(Guid id);
    Task<(List<Inventory> Inventories, int TotalCount)> GetInventoriesAsync(int page, int pageSize, string? searchTerm = null);
}
