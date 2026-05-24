using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDishRepository
{
    Task<List<Dish>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    /// <summary>Fetch dishes with DishIngredients → Ingredient eager loaded.</summary>
    Task<List<Dish>> GetByIdsWithIngredientsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tính popularity (1–5) cho từng DishId dựa trên tổng Quantity đặt trong N ngày gần nhất.
    /// Trả về Dictionary[dishId, popularity].
    /// </summary>
    Task<Dictionary<int, int>> GetPopularityScoresAsync(IEnumerable<int> dishIds, int lookbackDays = 28, CancellationToken cancellationToken = default);

    Task<Dish?> GetByIdAsync(int id);
    Task<Dish?> GetByIdWithIngredientsAsync(int id);

    /// <summary>Danh sách phân trang. <paramref name="category"/>: lọc theo dish_categories.SlotKey (vd main, soup).</summary>
    Task<(List<Dish> Dishes, int TotalCount)> GetDishesAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null, string? category = null);

    /// <summary>Món thuộc một bản ghi <c>dish_categories</c> (đặt suất đơn vị).</summary>
    Task<(List<Dish> Dishes, int TotalCount)> GetByDishCategoryIdAsync(
        int dishCategoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<Dish> CreateAsync(Dish dish);
    Task<Dish> UpdateAsync(Dish dish);

    /// <summary>Xóa toàn bộ slot cũ và gán lại theo danh sách SlotKey (dish_categories.SlotKey).</summary>
    Task ReplaceDishDishCategoriesAsync(int dishId, IReadOnlyList<string> categoryCodes, CancellationToken cancellationToken = default);

    /// <summary>Tra Id từ <c>cooking_methods.MethodKey</c> (enum AI), không phân biệt hoa thường.</summary>
    Task<int?> ResolveCookingMethodIdByMethodKeyAsync(string methodKey, CancellationToken cancellationToken = default);

    /// <summary>Món active có ít nhất một SlotKey trong danh sách (dùng cho gợi ý đi kèm).</summary>
    Task<List<Dish>> GetActiveDishesForSlotKeysAsync(
        IReadOnlyList<string> slotKeys,
        int excludeDishId,
        int limit,
        CancellationToken cancellationToken = default);

    /// <summary>Toàn bộ món active (fallback khi lọc slot không ra kết quả).</summary>
    Task<List<Dish>> GetActiveDishesForPairingPoolAsync(
        int excludeDishId,
        int limit,
        CancellationToken cancellationToken = default);

    /// <summary>Số lần món candidate xuất hiện cùng ngày/buổi với anchor trên thực đơn tuần.</summary>
    Task<Dictionary<int, int>> GetMenuCoOccurrenceCountsAsync(
        int anchorDishId,
        IEnumerable<int> candidateIds,
        CancellationToken cancellationToken = default);
}
