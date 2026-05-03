namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Liên kết món ↔ nhiều slot (bảng dish_dish_categories).
/// </summary>
public class DishDishCategory
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int DishId { get; set; }
    public int DishCategoryId { get; set; }

    public virtual Dish Dish { get; set; } = null!;
    public virtual DishCategory DishCategory { get; set; } = null!;
}
