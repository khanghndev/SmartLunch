namespace SmartLunch.Backend.Service.Domain.Entities;

public class IngredientActualIntakeLine
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int IntakeId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }

    public virtual IngredientActualIntake Intake { get; set; } = null!;
    public virtual Ingredient Ingredient { get; set; } = null!;
}
