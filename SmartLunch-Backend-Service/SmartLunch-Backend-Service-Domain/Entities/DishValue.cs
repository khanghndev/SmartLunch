namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Mức giá suất ăn (định mức BOM và hợp đồng B2B).
/// </summary>
public class DishValue
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public decimal Amount { get; set; }
    public string? Label { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public virtual ICollection<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();
}
