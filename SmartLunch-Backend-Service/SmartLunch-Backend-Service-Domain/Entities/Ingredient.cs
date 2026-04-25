namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Ingredient catalog
/// </summary>
public class Ingredient
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty; // kg, l, etc.
    public string? Description { get; set; }
    public int? DefaultSupplierId { get; set; }
    public decimal? CostPerUnit { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Partner? DefaultSupplier { get; set; }
    public virtual ICollection<IngredientSource> IngredientSources { get; set; } = new List<IngredientSource>();
    public virtual Inventory? Inventory { get; set; }
    public virtual ICollection<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();
}
