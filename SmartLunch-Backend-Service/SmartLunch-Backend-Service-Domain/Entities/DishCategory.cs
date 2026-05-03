namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Danh mục slot bữa ăn (bảng dish_categories) — khớp meal_structure / AI enum.
/// </summary>
public class DishCategory
{
    public int Id { get; set; }
    /// <summary>Mã nghiệp vụ (prefix + sequence, trigger).</summary>
    public string? Code { get; set; }
    /// <summary>Khóa slot AI: main, side, soup, … khớp DishSlotCategoryCodes / payload AI.</summary>
    public string SlotKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<DishDishCategory> DishDishCategories { get; set; } = new List<DishDishCategory>();
}
