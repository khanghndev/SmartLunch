namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Dish-Ingredient bill of materials
/// </summary>
public class DishIngredient
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int DishId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }

    public virtual Dish Dish { get; set; } = null!;
    public virtual Ingredient Ingredient { get; set; } = null!;
}
