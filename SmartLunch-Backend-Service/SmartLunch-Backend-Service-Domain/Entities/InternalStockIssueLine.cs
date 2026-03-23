namespace SmartLunch.Backend.Service.Domain.Entities;

public class InternalStockIssueLine
{
    public Guid Id { get; set; }
    public Guid IssueId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }

    public virtual InternalStockIssue Issue { get; set; } = null!;
    public virtual Ingredient Ingredient { get; set; } = null!;
}
