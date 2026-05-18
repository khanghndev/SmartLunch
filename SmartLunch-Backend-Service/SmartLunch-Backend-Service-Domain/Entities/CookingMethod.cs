namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Phương pháp chế biến (bảng cooking_methods) — MethodKey khớp enum AI.
/// </summary>
public class CookingMethod
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string MethodKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
