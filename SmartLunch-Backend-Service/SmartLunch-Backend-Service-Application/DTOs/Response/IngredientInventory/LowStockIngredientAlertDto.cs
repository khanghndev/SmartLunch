namespace SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

public class LowStockIngredientAlertDto
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal QuantityAvailable { get; set; }
    public decimal? ReorderLevel { get; set; }
    public decimal? Shortage { get; set; }
    public DateTime LastUpdated { get; set; }
}
