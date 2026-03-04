namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Menu schedule by day (dish assigned to a date and meal slot)
/// </summary>
public class MenuSchedule
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public DateTime Date { get; set; }
    public string MealSlot { get; set; } = "lunch"; // lunch, dinner, etc.
    public Guid DishId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual WeeklyMenu Menu { get; set; } = null!;
    public virtual Dish Dish { get; set; } = null!;
}
