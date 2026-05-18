namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Menu schedule by day (dish assigned to a date and meal slot)
/// </summary>
public class MenuSchedule
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int MenuId { get; set; }
    public DateTime Date { get; set; }
    public string MealSlot { get; set; } = "lunch"; // lunch, dinner, etc.
    public int DishId { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual WeeklyMenu Menu { get; set; } = null!;
    public virtual Dish Dish { get; set; } = null!;
}
