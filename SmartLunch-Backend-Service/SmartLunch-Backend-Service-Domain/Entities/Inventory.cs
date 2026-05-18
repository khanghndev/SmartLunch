namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Current ingredient stock level (1:1 with Ingredient)
/// </summary>
public class Inventory
{
    public int IngredientId { get; set; }
    public decimal QuantityAvailable { get; set; } = 0;
    public decimal? ReorderLevel { get; set; }
    public DateTime LastUpdated { get; set; } = VietnamTime.Now;

    public virtual Ingredient Ingredient { get; set; } = null!;
}
