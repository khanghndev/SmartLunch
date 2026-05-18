namespace SmartLunch.Backend.Service.Domain.Entities;

public class IngredientCategory
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty; // protein, seafood, vegetable, etc.
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
}
