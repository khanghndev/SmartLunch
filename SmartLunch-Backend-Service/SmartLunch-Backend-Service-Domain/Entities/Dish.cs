namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Dish (menu item)
/// </summary>
public class Dish
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; } // vegan, gluten-free, etc.
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();
    public virtual ICollection<MenuSchedule> MenuSchedules { get; set; } = new List<MenuSchedule>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
