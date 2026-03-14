namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Inventories;

public class InventoryDto
{
    public Guid IngredientId { get; set; }
    public decimal QuantityAvailable { get; set; }
    public decimal? ReorderLevel { get; set; }
    public DateTime LastUpdated { get; set; }
}
