using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(Guid id);
    Task<(List<Inventory> Inventories, int TotalCount)> GetInventoriesAsync(int page, int pageSize, string? searchTerm = null);

    /// <summary>
    /// Nguyên liệu đang hoạt động có mức tồn &lt;= ReorderLevel (cần cảnh báo).
    /// </summary>
    Task<IReadOnlyList<Inventory>> GetLowStockForActiveIngredientsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tồn kho kèm nguyên liệu và NCC mặc định (đọc chi tiết).
    /// </summary>
    Task<Inventory?> GetByIngredientIdWithIngredientAsync(Guid ingredientId, CancellationToken cancellationToken = default);
}
