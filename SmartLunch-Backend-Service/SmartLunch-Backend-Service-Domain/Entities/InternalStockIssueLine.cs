namespace SmartLunch.Backend.Service.Domain.Entities;

public class InternalStockIssueLine
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int IssueId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }

    public virtual InternalStockIssue Issue { get; set; } = null!;
    public virtual Ingredient Ingredient { get; set; } = null!;
}
