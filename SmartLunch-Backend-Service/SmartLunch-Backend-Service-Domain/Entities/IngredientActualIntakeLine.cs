namespace SmartLunch.Backend.Service.Domain.Entities;

public class IngredientActualIntakeLine
{
    public Guid Id { get; set; }
    public Guid IntakeId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }

    public virtual IngredientActualIntake Intake { get; set; } = null!;
    public virtual Ingredient Ingredient { get; set; } = null!;
}
